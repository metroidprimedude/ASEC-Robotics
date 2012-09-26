#include "_Spi.h"

SpiDevice::SpiDevice() {}

void SpiDevice::begin() { begin(SS); }

void SpiDevice::begin(byte selectPin) 
{
    _selectPin = selectPin;            
    _initPins();
    _initSpi();
}

void SpiDevice::_initPins() 
{
    pinMode(MOSI, OUTPUT);
    pinMode(MISO, INPUT);
    pinMode(SCK, OUTPUT);
    pinMode(_selectPin, OUTPUT);
    
    deselect();
}

void SpiDevice::_initSpi() 
{
    SPCR = (1<<SPE)|(1<<MSTR)|(1<<SPR1)|(1<<SPR0);
}

void SpiDevice::deselect() { digitalWrite(_selectPin, HIGH); }

void SpiDevice::select() { digitalWrite(_selectPin, LOW); }

byte SpiDevice::transfer(volatile byte data) 
{
    SPDR = data; // Start the transmission
    while (!(SPSR & (1<<SPIF)))
	{
        // Wait for the end of the transmission
    };
    return SPDR;    // Return the received byte
}

void SpiDevice::transfer_bulk(const uint8_t* srcptr, unsigned long int length)
{
    for(unsigned long int offset = 0; offset < length; offset++)
        transfer(srcptr[offset]);    }
}