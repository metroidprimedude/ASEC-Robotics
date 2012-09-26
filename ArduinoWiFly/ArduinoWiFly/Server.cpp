#include "WiFly.h"

#define NO_CLIENT Client ((uint8_t*) NULL, 0)

Server::Server(uint16_t port) : activeClient(NO_CLIENT)
{
    _port = port;
    WiFly.serverPort = port;
}

void Server::begin() {}

#define TOKEN_MATCH_OPEN "*OPEN*"

Client& Server::available() 
{
    if (!WiFly.serverConnectionActive)
        activeClient._port = 0;

    if (!activeClient) 
	{
        if (WiFly.uart.available() >= strlen(TOKEN_MATCH_OPEN)) 
		{
            if (WiFly.responseMatched(TOKEN_MATCH_OPEN))
			{
				activeClient._port = _port;
				activeClient._domain = NULL;
				activeClient._ip = NULL;

				activeClient.connect();
				WiFly.serverConnectionActive = true;
            } 
			else 
				WiFly.uart.flush();
        }
    }

    return activeClient;
}
