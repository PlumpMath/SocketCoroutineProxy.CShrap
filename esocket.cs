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

	private string ip;
	private int port;

	private bool RecvRunning;

	public IAsyncResult BeginConnect(AsyncCallback IFinished) {
		Debug.Log ("esocket start connect");
		if (socket == null) {
			CFinished = IFinished;
			return new startTcpWithTimeout ().BeginConnectWithTimeout (ip, port, FinishConnect, 2000);
		}
		AsyncAlreadyCompleted result = new AsyncAlreadyCompleted (socket);
		CFinished (result);
		return result;
	}

	void FinishConnect(IAsyncResult iar) {
		Debug.Log ("esocket end connect");
		Socket socket = (Socket)(iar.AsyncState);

		Debug.Log ("esocket end connect, state: " + iar.AsyncState);
		Debug.Log ("esocket end connect, result: " + socket.Connected);

		if (socket.Connected) {
			if (this.socket != null) {
				this.socket.Close ();
			}
			this.socket = socket;

			Recv_Start ();
		}
		CFinished (iar);
	}

	public void Send(string tosnd) {
		Debug.Log ("esocket start send");
		byte[] bs = System.Text.Encoding.UTF8.GetBytes (tosnd+"\n");
		socket.BeginSend(bs, 0, bs.Length, 0, new esocketSendCallback(pass).Callback, socket);
	}

	public void Send(byte[] tosnd, int offset, int len) {
		Debug.Log ("esocket start send");
		socket.BeginSend (tosnd, offset, len, 0, new esocketSendCallback(pass).Callback, socket);
	}

	public void SendCallback(string tosend, AsyncCallback ac) {
		Debug.Log ("esocket start send");
		byte[] bs = System.Text.Encoding.UTF8.GetBytes (tosend+"\n");
		socket.BeginSend(bs, 0, bs.Length, 0, new esocketSendCallback(ac).Callback, socket);
	}

	void pass(IAsyncResult iar) {
	}

	private string recvTmp = "";
	private byte[] buffer;
	void Recv_Start() {
		datas = new LinkedList<string> ();
		recvTmp = "";
		buffer = new byte[10];
		socket.BeginReceive (buffer, 0, buffer.Length, 0, Recv_Callback, buffer);
		RecvRunning = true;
	}

	void Recv_Callback(IAsyncResult iar) {
		int len = socket.EndReceive (iar);
		Debug.Log("esocket : recv_callback recved " + len + " bytes : " + System.Text.Encoding.UTF8.GetString (buffer, 0, len));
		if (len <= 0) {
			RecvRunning = false;
			//OnConnectionDown
			return;
		}

		recvTmp += System.Text.Encoding.UTF8.GetString (buffer, 0, len);
		if (buffer [len - 1] == '\n') {
			lock (datas) {
				datas.AddLast (recvTmp.Substring(0, recvTmp.Length-2)); // \r\n
				recvTmp = "";
			}
		}

		socket.BeginReceive (buffer, 0, buffer.Length, 0, Recv_Callback, buffer);
	}

	public esocket(string ip, int port) {
		this.ip = ip;
		this.port = port;
	}
}
// K2aWPlfBqDxpRXq+grS2PljBqDwhntm+/v9/PlXBqDwBAIA+5z3ZvlXBqDy2kj+9