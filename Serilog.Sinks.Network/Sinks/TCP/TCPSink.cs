using System;
using System.IO;
using System.Net;
using System.Text;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.Network.Formatters;

namespace Serilog.Sinks.Network.Sinks.TCP
{
  public class TCPSink : ILogEventSink, IDisposable
  {
    private readonly ITextFormatter _formatter;
    private readonly TcpSocketWriter _socketWriter;

    public TCPSink(IPAddress ipAddress, int port, string tlsAddress = null)
    {
	  _socketWriter = new TcpSocketWriter(new IPEndPoint(ipAddress, port), tlsAddress);
      _formatter = new LogstashJsonFormatter();
    }

    public void Emit(LogEvent logEvent)
    {
      var sb = new StringBuilder();

      using (var sw = new StringWriter(sb))
        _formatter.Format(logEvent, sw);

      var result = sb.ToString();
      result = result.Replace("RenderedMessage", "message");
      _socketWriter.Enqueue(result);
    }

    public void Dispose()
    {
      _socketWriter.Dispose();
    }
  }
}