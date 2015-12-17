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

	public LinkedList<string> datas = new LinkedList<string>();

	AsyncCallback CFinished;

	public ZSocketSignal task, complete;

	private string ip;
	private int port;

	private Thread threadRecv;

	public IAsyncResult BeginConnect(AsyncCallback IFinished) {
		Debug.Log ("esocket start connect");
		if (socket == null) {
			CFinished = IFinished;
			return new startTcpWithTimeout ().BeginConnectWithTimeout (ip, port, Finished, 2000);
		}
		AsyncAlreadyCompleted result = new AsyncAlreadyCompleted (socket);
		CFinished (result);
		return result;
	}

	void Finished(IAsyncResult iar) {
		Debug.Log ("esocket end connect");
		Socket socket = (Socket)(iar.AsyncState);

		Debug.Log ("esocket end connect, state: " + iar.AsyncState);
		Debug.Log ("esocket end connect, result: " + socket.Connected);

		if (socket.Connected) {
			this.socket = socket;
			if (threadRecv == null || !threadRecv.IsAlive) {
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
		Debug.Log ("start recv daemon");
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
				//Debug.Log(tmp);
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