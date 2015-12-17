using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System;
using System.Threading;

class AsyncAlreadyCompleted : IAsyncResult {
	private object _state;

	object IAsyncResult.AsyncState { get { return _state; } }
	bool IAsyncResult.IsCompleted { get { return true; } }
	WaitHandle IAsyncResult.AsyncWaitHandle { get { return null; } }
	bool IAsyncResult.CompletedSynchronously { get { return false; } }

	public AsyncAlreadyCompleted (object obj) {
		this._state = obj;
	}
}

public class esocket {
	private Socket socket;
	//private esocket instance;

	public LinkedList<string> datas = new LinkedList<string>();

	AsyncCallback CFinished;

	private string ip;
	private int port;

	private Thread threadRecv;

	public void Connect(AsyncCallback IFinished) {
		Debug.Log ("esocket start connect");
		if (socket == null) {
			CFinished = IFinished;
			new startTcpWithTimeout ().BeginConnectWithTimeout (ip, port, Finished, 2000);
		} else {
			CFinished (new AsyncAlreadyCompleted (socket));
		}
	}

	void Finished(IAsyncResult iar) {
		Debug.Log ("esocket end connect");
		socket = (Socket)(iar.AsyncState);

		if (socket.Connected) {
			//instance = this;
			if (!threadRecv.IsAlive) {
				threadRecv = new Thread (new ThreadStart (RecvDaemon));
				threadRecv.IsBackground = true;
				threadRecv.Start ();
			}
		}
		CFinished (iar);
	}

	public void Send(string tosnd) {
		Debug.Log ("esocket start send");
		byte[] bs = System.Text.Encoding.UTF8.GetBytes (tosnd+"\n");
		socket.BeginSend(bs, 0, bs.Length, 0, pass, socket);
	}

	public void Send(byte[] tosnd, int offset, int len) {
		Debug.Log ("esocket start send");
		socket.BeginSend (tosnd, offset, len, 0, pass, socket);
	}

	public void SendCallback(string tosend, AsyncCallback ac) {
		Debug.Log ("esocket start send");
		byte[] bs = System.Text.Encoding.UTF8.GetBytes (tosend+"\n");
		socket.BeginSend(bs, 0, bs.Length, 0, ac, socket);
	}

	void pass(IAsyncResult iar) {
		Debug.Log ("esocket end send");
	}

	void RecvDaemon() {
		string tmp = "";
		byte[] buffer = new byte[10];
	
		for (;;) {
			int len = socket.Receive (buffer);

			//Debug.Log ("len : " + len);
			//Debug.Log ("Buffer : " + System.Text.Encoding.UTF8.GetString (buffer, 0, len));

			if (len <= 0) {
				//OnConnectionDown
				break;
			}

			tmp += System.Text.Encoding.UTF8.GetString (buffer, 0, len);
			if (buffer [len - 1] == '\n') {
				lock (datas) {
					datas.AddLast (tmp);
				}
				//OnRecv
				Debug.Log(tmp);
				tmp = "";
			}
		}
	}

	public esocket(string ip, int port) {
		this.ip = ip;
		this.port = port;
	}
}
// K2aWPlfBqDxpRXq+grS2PljBqDwhntm+/v9/PlXBqDwBAIA+5z3ZvlXBqDy2kj+9