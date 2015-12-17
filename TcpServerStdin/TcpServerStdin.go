package main

import  (
	"os"
	"net"
	log "github.com/Sirupsen/logrus"
	"bufio"
	"container/list"
	"sync"
	"fmt"
)

func main() {
	// Log as JSON instead of the default ASCII formatter.
	log.SetFormatter(&log.TextFormatter{})

	// Output to stderr instead of stdout, could also be a file.
	log.SetOutput(os.Stderr)

	// Only log the warning severity or above.
	log.SetLevel(log.DebugLevel)

	service := ":9988"
	tcpAddr, err := net.ResolveTCPAddr("tcp4", service)
	if err != nil {
		log.Error("Can not resolveTcpAddr : " + err.Error())
		return
	}
	listener, err := net.ListenTCP("tcp", tcpAddr)
	if err != nil {
		log.Error("Can not ListenTco : " + err.Error())
		return
	}

	for {
		conn, err := listener.Accept()
		if err != nil {
			continue
		}
		go handleClient(conn)
	}
}

type Tcp struct {
	Conn net.Conn
	In chan string
	Out chan string

	Data *list.List
	Lock sync.RWMutex
}

func handleRecv(tcp *Tcp) {
	conn := tcp.Conn

	request := make([]byte, 10)

	tmp := ""

	for {
		read_len, err := conn.Read(request)

		if err != nil {
			log.Error(err.Error())
			break
		}

		if read_len <= 0 {
			log.Debug("read_len == 0")
			break
		} else {
			tmp = tmp + string(request[:read_len])
			if request[read_len-1] == '\n' {
				tcp.Lock.Lock()
				tcp.Data.PushBack(tmp)
				tcp.Lock.Unlock()

				tmp = ""
			}
			log.Debugf("Recv Package %d %v [%s]\n", read_len, request[:read_len], request)
		}

		request = make([]byte, 10)
	}
}

func handleSend(tcp *Tcp) {
	conn := tcp.Conn

	tmp := ""

	for {
		tmp = <- tcp.Out

		if len(tmp) == 0 || tmp[len(tmp)-1] != '\n' {
			tmp += "\r\n"
		}

		n, err := conn.Write(([]byte)(tmp))

		if err != nil {
			log.Error(err.Error())
			continue
		}

		log.Debugf("Send Package len:%d [%s]\n", n, tmp)
	}
}

func handleClient(conn net.Conn) {
	tcp := &Tcp {
		Conn:conn,
		In:make(chan string),
		Out:make(chan string),
		Data:list.New(),
	}

	go handleRecv(tcp)
	go handleSend(tcp)

	stdin := bufio.NewReader(os.Stdin)

	for {
		if line, _, err := stdin.ReadLine(); err == nil {
			tcp.Out <- (string)(line)
		} else {
			log.Debug(err.Error())
			break
		}

		tcp.Lock.Lock()
		for tcp.Data.Len() > 0 {
			fmt.Printf("%s\n", tcp.Data.Front().Value.(string))
			tcp.Data.Remove(tcp.Data.Front())
		}
		tcp.Lock.Unlock()
	}
}