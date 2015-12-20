using UnityEngine;
using System.Collections;
using System;

public class ZSocketSignal {
	public enum Signals
	{
		ConnectFailed,
		ConnectSuccessful,
		RecvSuccessful,
		RecvFailed,
		SendSuccessful,
		SendFailed,
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

	public void Update(Signals sig) {
		realSignal = sig;
		realValue = "";
	}

	public void Update(Signals sig, string val) {
		realSignal = sig;
		realValue = val;
	}

	public ZSocketSignal(Signals sig) {
		realSignal = sig;
		realValue = "";
	}

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

	public string[] Parse() {
		return realValue.Split (new string[] { " " }, StringSplitOptions.None);
	}
}