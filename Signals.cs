public enum SocketSignals
{
	ConnectFailed,
	ConnectSuccessful,
	RecvSuccessful,
	RecvFailed,
	SendSuccessful,
	SendFailed,
	SendAndRecvSuccessful,
	SendAndRecvFailed,
	Dead,

	Connect,
	ConnectBlock,
	Recv,
	Send,
	SendBlock,
	SendAndRecv
}