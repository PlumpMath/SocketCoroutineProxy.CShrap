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

	public IEnumerator Proxy() {
		while (true) {
			if (!ie.MoveNext ()) {
				yield break;
			}

			object yielded = ie.Current;

			if (yielded != null && yielded.GetType () == typeof(ZSocketSignal)) {
				ZSocketSignal Sig = (ZSocketSignal)yielded;

				//this should only use for heart beat
				if (Sig.Signal == ZSocketSignal.Signals.Send) {
					es.Send (Sig.Value);
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
					
				case ZSocketSignal.Signals.ConnectBlock:
					if (Connected) {
						Sig.Update (ZSocketSignal.Signals.ConnectSuccessful);
					} else {
						Sig.Update (ZSocketSignal.Signals.ConnectFailed);
					}
					break;

				case ZSocketSignal.Signals.Recv:
					if (!Connected) {
						Sig.Update (ZSocketSignal.Signals.RecvFailed);
						break;
					}

					while (true) {
						bool esDatasWithNull = true;
						//Debug.Log ("socketproxy Signals.Recv es.datas.Count " + es.datas.Count);
						lock (es.datas) {
							if (es.datas.Count > 0) {
								esDatasWithNull = false;
							}
						}
						if (esDatasWithNull) {
							yield return null;
						} else {
							lock (es.datas) {
								string data = es.datas.First.Value;
								es.datas.RemoveFirst ();
								Sig.Update (ZSocketSignal.Signals.RecvSuccessful, data);
								//Callback (new ZSocketSignal (ZSocketSignal.Signals.Recv, data));
							}
							break;
						}
					}
					break;

				case ZSocketSignal.Signals.SendBlock:
					es.SendCallback (Sig.Value, SendCallback);
					Sended = false;
					while (!Sended) {
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
