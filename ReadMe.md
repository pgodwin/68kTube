# 68kTube Alpha

68kTube is a client/server application which allows Macitosh Computers with QuickTime 4.1 or later to stream video from YouTube.

What Works:
 - Searching
 - Transcoding
 - Playing Videos

Installing
==========
There is no binary distribution yet. However it should compile cleanly in Visual Studio 2015. 

Requirements
============
Mac Side:
At the moment, despite it's name a PPC Mac running Quicktime 4.1 or later and MacOs 7.1. You will also need OpenTransport installed and configured.

Server Side:
 - Windows Server
 - .NET 4.6
 - QuickTime 7
 - Lots of processing power

Debugging in Visual Studio
===========================
If your base-address is bound to all interfaces (the default - http://*), you will need to run Visual Studio as an Administrator to debug the server.

Tech
====
Technology Used (Server Side):
  - FFMPEG
  - QuickTime
  - C# Base media File Format (https://basemedia.codeplex.com/)

Technology Used (Client Side):
  - RealBasic 2.1.2
  - HttpSocket 2.0 (http://www3.nd.edu/~jvanderk/rb/httpsocket.html)

