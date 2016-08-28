using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media;
using MatrixIO.IO.MpegTs;
using MatrixIO.IO.MpegTs.Streams;

namespace SLTest
{
    public enum NalUnitTypeCode
    {
        Unspecified = 0,
        SliceLayerWithoutPartitioningNonIDR = 1,
        SliceDataPartitionLayerA = 2,
        SliceDataPartitionLayerB = 3,
        SliceDataPartitionLayerC = 4,
        SliceLayerWithoutPartitioningIDR = 5,
        SupplementalEnahancementInformation = 6,
        SequanceParameterSet = 7,
        PictureParameterSet = 8,
        AccessUnitDelimiter = 9,
        EndOfSequence = 10,
        EndOfStream = 11,
        FillerData = 12,
        // 13..23 Reserved
        // 24..31 Unspecified
    }

    public class TestStreamSource : MediaStreamSource
    {
        private readonly object _syncObject = new object();
        private readonly TsStream _stream;
        private readonly Queue<TsStreamEventArgs> _sampleQueue = new Queue<TsStreamEventArgs>();
        private readonly AutoResetEvent _sampleArrivedEvent = new AutoResetEvent(false);

        private readonly MediaStreamDescription _streamDescription;
        private Dictionary<MediaSourceAttributesKeys, string> _mediaSourceAttributes;
        private readonly Dictionary<MediaStreamAttributeKeys, string> _mediaStreamAttributes;
        private Dictionary<MediaSampleAttributeKeys, string> _mediaSampleAttributes;

        public TestStreamSource(TsStream stream)
        {
            _stream = stream;

            _mediaSourceAttributes = new Dictionary<MediaSourceAttributesKeys, string>();
            _mediaStreamAttributes = new Dictionary<MediaStreamAttributeKeys, string>();
            _mediaSampleAttributes = new Dictionary<MediaSampleAttributeKeys, string>();

            MediaStreamType type;
            switch(_stream.Type)
            {
                case StreamType.MPEG4_AVC:
                    type = MediaStreamType.Video;
                    _mediaStreamAttributes.Add(MediaStreamAttributeKeys.VideoFourCC, "H264");
                    _mediaStreamAttributes.Add(MediaStreamAttributeKeys.Width, "1280");
                    _mediaStreamAttributes.Add(MediaStreamAttributeKeys.Height, "720");
                    break;
                case StreamType.MPEG4_AAC:
                    type = MediaStreamType.Audio;
                    break;
                default:
                    return;
            }

            _streamDescription = new MediaStreamDescription(type, _mediaStreamAttributes);
        }

        private byte[] _sps;
        private byte[] _pps;

        void UnitReceived(object sender, TsStreamEventArgs e)
        {
            _sampleQueue.Enqueue(e);

            if (!(e is TsStreamEventArgs<PesPacket>) || (_sps != null && _pps != null)) return;

            var pesPacket = ((TsStreamEventArgs<PesPacket>) e).DecodedUnit;
            if (pesPacket.Data == null) return;

            var position = 0;
            int nalUnitStart = 0;
            var nalUnitType = NalUnitTypeCode.Unspecified;
            var foundNalUnit = false;
            while (position + 3 < pesPacket.Data.Length && (_sps == null || _pps == null))
            {
                if (pesPacket.Data[position] == 0x00 && pesPacket.Data[position + 1] == 0x00 && 
                    pesPacket.Data[position + 2] == 0x00 && pesPacket.Data[position + 3] == 0x01)
                {
                    var type = (NalUnitTypeCode)(pesPacket.Data[position + 4] & 0x1F);
                    Debug.WriteLine("Found " + type + " NAL Unit");

                    if (foundNalUnit)
                    {
                        var nalUnit = new byte[position - (nalUnitStart + 4)];
                        Buffer.BlockCopy(pesPacket.Data, nalUnitStart + 4, nalUnit, 0, nalUnit.Length);
                        switch(nalUnitType)
                        {
                            case NalUnitTypeCode.PictureParameterSet:
                                _pps = nalUnit;
                                break;
                            case NalUnitTypeCode.SequanceParameterSet:
                                _sps = nalUnit;
                                break;
                        }
                    }
                    if (type == NalUnitTypeCode.SequanceParameterSet || type == NalUnitTypeCode.PictureParameterSet)
                    {
                        foundNalUnit = true;
                        nalUnitStart = position;
                        nalUnitType = type;
                    }
                    position += 4;
                }
                else position++;
            }
            if (_sps != null && _pps != null)
            {
                StringBuilder sb = new StringBuilder("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ ");
                foreach (var b in _sps) sb.Append(b.ToString("X2"));
                foreach (var b in _pps) sb.Append(b.ToString("X2"));
                Debug.WriteLine(sb.ToString());

                var codecPrivateData = Convert.ToBase64String(_sps) + "," + Convert.ToBase64String(_pps);
                _streamDescription.MediaAttributes.Add(MediaStreamAttributeKeys.CodecPrivateData, codecPrivateData);
                ReportOpenMediaCompleted(_mediaSourceAttributes, new[] {_streamDescription});
            }
        }

        protected override void CloseMedia()
        {
            _stream.UnitReceived -= UnitReceived;
            lock (_syncObject) _sampleQueue.Clear();
            _sampleArrivedEvent.Set();
        }

        protected override void GetDiagnosticAsync(MediaStreamSourceDiagnosticKind diagnosticKind)
        {
            throw new NotImplementedException();
        }


        private TsStreamEventArgs _sample;
        private List<ArraySegment<byte>> _sampleUnits;
        private int _currentSampleUnit;
        private MemoryStream _currentSampleStream;

        private List<ArraySegment<byte>> IndexNetworkAbstractionLayerUnits(TsStreamEventArgs<PesPacket> pesPacketArgs)
        {
            var nalUnits = new List<ArraySegment<byte>>();
            var pesPacket = pesPacketArgs.DecodedUnit;
            var position = 0;
            var nalUnitStart = 0;
            var havePreviousNalUnit = false;
            Debug.WriteLine("Parsing NAL Units:");
            while (position + 3 < pesPacket.Data.Length)
            {
                if (pesPacket.Data[position] == 0x00 && pesPacket.Data[position + 1] == 0x00 &&
                    pesPacket.Data[position + 2] == 0x00 && pesPacket.Data[position + 3] == 0x01)
                {
                    if (havePreviousNalUnit)
                    {
                        nalUnits.Add(new ArraySegment<byte>(pesPacket.Data, nalUnitStart, position - nalUnitStart));
                        Debug.WriteLine("\tStart: " + nalUnitStart + " Length: " + (position - nalUnitStart));
                    }
                    else havePreviousNalUnit = true;
                    
                    // NOTE: Not sure if this should be before or after.  Neither works at present.  Assuming after for now because the extradata has the start codes stripped out.
                    nalUnitStart = position + 1;
                    position += 4;
                }
                else position++;
            }
            if (havePreviousNalUnit)
            {
                nalUnits.Add(new ArraySegment<byte>(pesPacket.Data, nalUnitStart, position - nalUnitStart));
                Debug.WriteLine("\tStart: " + nalUnitStart + " Length: " + (position - nalUnitStart));
            }

            return nalUnits;
        }

        protected override void GetSampleAsync(MediaStreamType mediaStreamType)
        {
            switch(mediaStreamType)
            {
                case MediaStreamType.Audio:
                    break;
                case MediaStreamType.Video:
                    GetVideoSampleAsync();
                    break;
                case MediaStreamType.Script:
                    break;
            }
        }

        private bool _parametersSent;

        private void GetVideoSampleAsync()
        {
            MemoryStream stream;
            int streamOffset;
            int streamLength;
            bool isKeyFrame;
            long pts;

            if (!_parametersSent)
            {
                stream = new MemoryStream();
                stream.Write(new byte[] { 0x00, 0x00, 0x01 }, 0, 3);
                stream.Write(_sps, 0, _sps.Length);
                stream.Write(new byte[] { 0x00, 0x00, 0x01 }, 0, 3);
                stream.Write(_pps, 0, _pps.Length);
                stream.Seek(0, SeekOrigin.Begin);
                streamOffset = 0;
                streamLength = stream.Length;
                _parametersSent = true;
            }
            else
            {
                while (_sample == null)
                {
                    lock (_syncObject)
                    {
                        if (_sampleQueue.Count > 0)
                        {
                            _sample = _sampleQueue.Dequeue();

                            var tsStreamEventArgs = _sample as TsStreamEventArgs<PesPacket>;
                            if (tsStreamEventArgs != null)
                            {
                                _sampleUnits = IndexNetworkAbstractionLayerUnits(tsStreamEventArgs);
                                _currentSampleUnit = 0;
                            }
                        }
                    }
                    if (_sample == null) _sampleArrivedEvent.WaitOne();
                }

                if (_sample is TsStreamEventArgs<byte[]>)
                {
                    stream = new MemoryStream(((TsStreamEventArgs<byte[]>) _sample).DecodedUnit);
                    streamOffset = 0;
                    streamLength = (int) stream.Length;
                    isKeyFrame = true;
                    pts = 0;
                }
                else if (_sample is TsStreamEventArgs<PesPacket>)
                {
                    var pesPacket = ((TsStreamEventArgs<PesPacket>) _sample).DecodedUnit;
                    if (_currentSampleUnit == 0)
                        _currentSampleStream = new MemoryStream(pesPacket.Data);

                    stream = _currentSampleStream;
                    var sampleUnit = _sampleUnits[_currentSampleUnit];
                    streamOffset = sampleUnit.Offset;
                    streamLength = sampleUnit.Count;
                    pts = pesPacket.Header.PTS.HasValue ? pesPacket.Header.PTS.Value.Ticks : 0;

                    var type = (NalUnitTypeCode) (sampleUnit.Array[sampleUnit.Offset + 4] & 0x1F);
                    isKeyFrame = type == NalUnitTypeCode.SliceLayerWithoutPartitioningIDR;

                    _currentSampleUnit++;
                    if (_currentSampleUnit >= _sampleUnits.Count)
                    {
                        _sample = null;
                        _currentSampleUnit = 0;
                    }
                }
                else return;
            }
            /*
            var mediaSampleAttributes = new Dictionary<MediaSampleAttributeKeys, string>()
                                            {
                                                {MediaSampleAttributeKeys.KeyFrameFlag, isKeyFrame ? "1" : "0"},
                                                {MediaSampleAttributeKeys.FrameWidth, "1280"},
                                                {MediaSampleAttributeKeys.FrameHeight, "720"},
                                            };
            */
#if DEBUG2
            StringBuilder sb = new StringBuilder("@@@  ");
            stream.Seek(streamOffset, SeekOrigin.Begin);
            for (int i = 0; i < streamLength; i++)
            {
                byte b = (byte)stream.ReadByte();
                sb.Append(b.ToString("X2"));
            }
            Debug.WriteLine(sb.ToString());
            stream.Seek(streamOffset, SeekOrigin.Begin);
#endif
            ReportGetSampleCompleted(new MediaStreamSample(_streamDescription, stream, streamOffset,
                                                           streamLength, pts, _mediaSampleAttributes));
        }

        protected override void OpenMediaAsync()
        {
            _stream.UnitReceived += UnitReceived;
            _mediaSourceAttributes.Clear();
            _mediaSourceAttributes.Add(MediaSourceAttributesKeys.CanSeek, "False");
            _mediaSourceAttributes.Add(MediaSourceAttributesKeys.Duration, "0");
        }

        protected override void SeekAsync(long seekToTime)
        {
            ReportSeekCompleted(seekToTime);
        }

        protected override void SwitchMediaStreamAsync(MediaStreamDescription mediaStreamDescription)
        {
            throw new NotImplementedException();
        }
    }
}
