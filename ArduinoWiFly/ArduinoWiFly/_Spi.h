#ifndef ___SPI_H__
#define ___SPI_H__

#include <WProgram.h>
#include <pins_arduino.h>

class SpiDevice 
{
  public:
    SpiDevice();
    
    void begin();
    void begin(byte selectPin);

    void deselect();
    void select();

    byte transfer(volatile byte data);
    void transfer_bulk(const uint8_t* srcptr, unsigned long int length);

    
  private:
    void _initPins();
    void _initSpi();    
  
    byte _selectPin;
    
};

#endif

