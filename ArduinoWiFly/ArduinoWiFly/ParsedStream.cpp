#include "ParsedStream.h"

void ParsedStream::storeByte(unsigned char c) 
{
    int i = (_rx_buffer.head + 1) % RX_BUFFER_SIZE;

    if (i != _rx_buffer.tail) 
	{
        _rx_buffer.buffer[_rx_buffer.head] = c;
        _rx_buffer.head = i;
    }
}

ParsedStream::ParsedStream(SpiUartDevice& uart) : _uart(uart) { reset(); }

void ParsedStream::reset() 
{
    ring_buffer _rx_buffer = { { 0 }, 0, 0};
    _closed = false;
    bytes_matched = 0;    
}

uint8_t ParsedStream::available(bool raw) 
{
    uint8_t available_bytes;
    
    available_bytes = (RX_BUFFER_SIZE + _rx_buffer.head - _rx_buffer.tail) % RX_BUFFER_SIZE;

    if (!raw) 
	{
        if (available_bytes > bytes_matched) 
            available_bytes -= bytes_matched;
        else 
            available_bytes = 0;
    }

    return available_bytes;
}

uint8_t ParsedStream::available() 
{
    while (!_closed && freeSpace() && _uart.available())
        getByte();

    return available(false);
}

bool ParsedStream::closed() 
{
    return _closed && !available();
}

int ParsedStream::read(void) 
{
    if (!available())
        getByte();

    if (!available())
        return -1;
	else 
	{
        unsigned char c = _rx_buffer.buffer[_rx_buffer.tail];
        _rx_buffer.tail = (_rx_buffer.tail + 1) % RX_BUFFER_SIZE;
        return c;
    }
}

int ParsedStream::freeSpace() 
{
    return RX_BUFFER_SIZE - available(true) - 1;
}

void ParsedStream::getByte() 
{
    int c;

    if (_closed)
        return;

    if (freeSpace() == 0)
        return;

    c = _uart.read();

    if (c == -1)
        return;

    if (c == MATCH_TOKEN[bytes_matched])
	{
        bytes_matched++;
        if (bytes_matched == strlen(MATCH_TOKEN))
            _closed = true;
    } 
	else if (c == MATCH_TOKEN[0])
        bytes_matched = 1;
    else
        bytes_matched = 0;

    storeByte(c);    
}