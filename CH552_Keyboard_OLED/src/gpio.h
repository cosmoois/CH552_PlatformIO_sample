#ifndef __GPIO_H
#define __GPIO_H

#include "ch554_platform.h"

// #define GPIO_INTERRUPT  1

SBIT(P1_0, 0x90, 0);
SBIT(P1_1, 0x90, 1);
// SBIT(P1_2, 0x90, 2);    // Port not connected
// SBIT(P1_3, 0x90, 3);    // Port not connected
SBIT(P1_4, 0x90, 4);
SBIT(P1_5, 0x90, 5);
SBIT(P1_6, 0x90, 6);
SBIT(P1_7, 0x90, 7);

// SBIT(P3_0, 0xB0, 0);    // on board LED
SBIT(P3_1, 0xB0, 1);
SBIT(P3_2, 0xB0, 2);
// SBIT(P3_3, 0xB0, 3);    // SCL (I2C)
// SBIT(P3_4, 0xB0, 4);    // SDA (I2C)
// SBIT(P3_5, 0xB0, 5);    // Timer used in uart
// SBIT(P3_6, 0xB0, 6);    // UDP (USB)
// SBIT(P3_7, 0xB0, 7);    // UDM (USB)

void Port1Cfg(uint8_t mode, uint8_t Pin);
void Port3Cfg(uint8_t mode, uint8_t Pin);
void GPIOInterruptCfg();

#endif
