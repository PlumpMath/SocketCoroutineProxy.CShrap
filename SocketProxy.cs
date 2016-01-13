using UnityEngine;
using System.Collections;
using System;
using System.Net.Sockets;

// scripts access esocket coroutinely via this script
// proxy socket from async to coroutine
// not muti thread safe
public class SocketProxy {
	private bool Connected;
	private bool Connecting;
	private bool Sended;

	private IEnumerator ie;
	public  esocket es;
	//private SocketCallback Callback;

	public delegate void SocketCallback(ZSocketSignal zsc);

	/*
	public SocketProxy(IEnumerator ie, SocketCallback Callback) {
		this.ie = ie;
		this.Callback = Callback;

		this.es = new esocket ("127.0.0.1", 9988);
	}
	*/

	public SocketProxy(IEnumerator ie, /*SocketCallback Callback, */esocket es) {
		this.ie = ie;
		//this.Callback = Callback;
		this.es = es;
	}

	void SendCallback(IAsyncResult iar) {
		Sended = true;
	}

	void ConnectCallback(IAsyncResult iar) {
		Socket so = (Socket)iar.AsyncState;
		/*
		if (Connected) {
			Callback (new ZSocketSignal (ZSocketSignal.Signals.ConnectSuccessful, ""));
		} else {
			Callback (new ZSocketSignal (ZSocketSignal.Signals.ConnectFailed, ""));
		}
		*/
		Connecting = false;
		Connected = so.Connected;
	}

	void Connect() {
		if (!Connected && !Connecting) {
			Connecting = true;
			es.BeginConnect (ConnectCallback);
		}
	}

	string Recv() {
		lock (es.datas) {
			if (es.datas.Count <= 0) {
				return null;
			}
			string data = es.datas.First.Value;
			es.datas.RemoveFirst ();
			return data;
		}
	}

	public IEnumerator Proxy() {
		while (true) {
			if (!ie.MoveNext ()) {
				yield break;
			}

			object yielded = ie.Current;

			if (yielded != null && yielded.GetType () == typeof(ZSocketSignal)) {
				ZSocketSignal Sig = (ZSocketSignal)yielded;

				//this should only use for heart beat
				if (Sig.Signal == SocketSignals.Send) {
					es.Send (Sig.ValueString);
					yield return null;
					continue;
				}

				//else all method should check connection
				Connect ();
				do {
					yield return null;
				} while (Connecting);

				switch (Sig.Signal) {

				//this should not be called. Connect should always blocked
				//case ZSocketSignal.Signals.Connect:
				//	Connect ();
				//	yield return null;
				//	break;
					
				case SocketSignals.ConnectBlock:
					if (Connected) {
						Sig.Update (SocketSignals.ConnectSuccessful);
					} else {
						Sig.Update (SocketSignals.ConnectFailed);
					}
					break;

				case SocketSignals.SendBlock:
					if (!Connected) {
						Sig.Update (SocketSignals.RecvFailed);
						break;
					}

					es.SendCallback (Sig.ValueString, SendCallback);
					Sended = false;
					while (!Sended) {
						yield return null;
					}
					Sig.Update (SocketSignals.SendSuccessful);
					break;
				
				case SocketSignals.Recv:
					if (!Connected) {
						Sig.Update (SocketSignals.RecvFailed);
						break;
					}

					while (true) {
						string recv = Recv ();
						if (recv != null) {
							Sig.Update (SocketSignals.RecvSuccessful, recv);
							break;
						}
						yield return null;
					}
					break;

				case SocketSignals.SendAndRecv:
					if (!Connected) {
						Sig.Update (SocketSignals.RecvFailed);
						break;
					}

					es.SendCallback (Sig.ValueString, SendCallback);
					Sended = false;
					while (!Sended) {
						yield return null;
					}
					while (true) {
						string recv = Recv ();
						if (recv != null) {
							Sig.Update (SocketSignals.SendAndRecvSuccessful, recv);
							break;
						}
						yield return null;
					}
					break;
				}

				
			} else {
				yield return yielded;
			}
		}
	}
}
