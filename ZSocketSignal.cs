using UnityEngine;
using System.Collections;

public class ZSocketSignal {
	public enum Signals
	{
		ConnectFailed,
		ConnectSuccessful,
		Dead,

		Connect,
		ConnectBlock,
		Recv,
		Send,
		SendBlock
	}

	public Signals Signal {
		get {
			return realSignal;
		}
	}

	public string Value {
		get {
			return realValue;
		}
	}

	private string realValue;
	private Signals realSignal;

	public ZSocketSignal(Signals sig, string cmd) {
		realSignal = sig;
		realValue = cmd;
	}

	public ZSocketSignal(Signals sig, string cmd, string arg) {
		realSignal = sig;
		realValue = cmd + " " + arg;
	}

	public ZSocketSignal(Signals sig, string cmd, string arg1, string arg2) {
		realSignal = sig;
		realValue = cmd + " " + arg1 + " " + arg2;
	}

	public ZSocketSignal(Signals sig, string cmd, string[] args) {
		realSignal = sig;
		realValue = cmd;
		foreach (string arg in args) {
			realValue += " " + arg;
		}
	}
}