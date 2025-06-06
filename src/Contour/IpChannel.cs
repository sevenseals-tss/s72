﻿using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace SevenSeals.Tss.Contour;

public class IpChannel : Channel
{
    private string host;
    private ushort port;
    private Socket _comm;
    private string _id;

    public IpChannel(IChannelEvents events, ushort responseTimeout, ushort aliveTimeout, ushort deadTimeout, string host_, ushort port_)
        : base(events, responseTimeout, aliveTimeout, deadTimeout)
    {
        host = host_;
        port = port_;
        _id = $"{host}:{port}";
    }

    public override void Dispose()
    {
        _fini();
        base.Dispose();
    }

    public override string Id { get; }
    public override string ConnInfo()
    {
        throw new NotImplementedException();
    }

    protected override void _init()
    {
        _comm = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var ipAddr = Dns.GetHostAddresses(host)[0];
        var endpoint = new IPEndPoint(ipAddr, port);
        _comm.Connect(endpoint);
        _comm.NoDelay = true;
        _setReady(true);
    }

    protected override void _fini()
    {
        _setReady(false);
        _comm?.Close();
        _comm = null;
    }

    protected override int _read(byte[] buffer, int size)
    {
        Debug.Assert(size > 0);
        int bytesRead = 0;

            if (!_waitInput(responseTimeout))
                return 0;
            bytesRead = _comm.Receive(buffer, 0, size, SocketFlags.None);

        return bytesRead;
    }

    protected override void _write(byte[] buffer, int size)
    {
        Debug.Assert(size > 0);
        _chkReady();

        if (_error)
            _flushInput();
        _comm.Send(buffer, 0, size, SocketFlags.None);
    }

    private bool _waitInput(int timeout)
    {
        return _comm.Poll(timeout * 1000, SelectMode.SelectRead);
    }
}
