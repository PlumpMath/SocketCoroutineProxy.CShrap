using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;


public class startTcpWithTimeout
{
	IAsyncResult result;

	private int tle;
	private AsyncCallback acb;
	private Thread thread;

	void pass(IAsyncResult iar) {
	}

	public IAsyncResult BeginConnectWithTimeout(Socket socket, IPEndPoint ip, AsyncCallback acb, object state, int tle) {
		this.tle = tle;
		this.acb = acb;

		result = socket.BeginConnect (ip, pass, state);

		thread = new Thread (new ThreadStart (watch));
		thread.IsBackground = true;
		thread.Start ();

		return result;
	}

	public IAsyncResult BeginConnectWithTimeout(IPEndPoint ip, AsyncCallback acb, int tle) {
		Socket socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		return BeginConnectWithTimeout (socket, ip, acb, socket, tle);
	}

	public IAsyncResult BeginConnectWithTimeout(string ip, int port, AsyncCallback acb, int tle) {
		IPEndPoint ipe = new IPEndPoint (IPAddress.Parse (ip), port);
		return BeginConnectWithTimeout (ipe, acb, tle);
	}

	public startTcpWithTimeout ()
	{
	}

	void watch ()
	{
		result.AsyncWaitHandle.WaitOne (tle, true);
		acb (result);
	}
}