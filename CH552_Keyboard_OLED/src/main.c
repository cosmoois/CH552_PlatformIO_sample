#include <stdio.h>
#include "ch554_platform.h"
#include <debug.h>
#include "usb_endp.h"
#include "usb_hid_keyboard.h"
#include "gpio.h"
#include "ssd1306_ascii.h"

#define LED_PIN 0
SBIT(LED, 0xB0, LED_PIN);

uint8_t keyState, kbdModifier, kbdKey;

void main(void) {
	uint8_t str[8];
	uint8_t x = 0;
	uint8_t y = 0;
	uint8_t p1_lat, p3_lat;

    CH554_Init();

    // Configure pin 1.0,1.1 1.4 - 1.7 as GPIO input & pullup
	Port3Cfg(3, 0);
	Port3Cfg(3, 1);
	Port3Cfg(3, 4);
	Port3Cfg(3, 5);
	Port3Cfg(3, 6);
	Port3Cfg(3, 7);

    // Configure pin 3.1 - 3.2 as GPIO input & pullup
	Port3Cfg(3, 1);
	Port3Cfg(3, 2);

    // Configure pin 3.0 as GPIO output
	Port3Cfg(1, LED_PIN);
	LED = 0;

	// for used UART0
    printf("");	// OLEDに等間隔で縦のノイズが入るため、UART0を有効にしてダミー送信を行っておくとなぜか解消する

	OLED_Init();
	OLED_P8x16Str(x, y, "_");

    keyState = KBD_STATE_IDLE;

    while(1) {
		while (1)
		{
			// 有効なポートのみを取得し、押下されたら1にする
			p1_lat = (~P1 & 0xF3);
			p3_lat = (~P3 & 0x06);
			// どれかが押下されていれば抜ける
			if ((p1_lat | p3_lat) != 0)	break;
		}
		LED = 1;
		// sprintf(str, "%02X, %02X", p1_lat, p3_lat);
		// OLED_P8x16Str(0, 2, str);
		if ((p1_lat & (1 << 4)) != 0)	kbdKey = 0x04;	// 'a'
		if ((p1_lat & (1 << 6)) != 0)	kbdKey = 0x05;	// 'b'
		if ((p3_lat & (1 << 2)) != 0)	kbdKey = 0x06;	// 'c'
		if ((p1_lat & (1 << 5)) != 0)	kbdKey = 0x07;	// 'd'
		if ((p1_lat & (1 << 7)) != 0)	kbdKey = 0x08;	// 'e'
		if ((p1_lat & (1 << 0)) != 0)	kbdKey = 0x09;	// 'f'
		if ((p3_lat & (1 << 1)) != 0)	kbdKey = 0x0A;	// 'g'
		if ((p1_lat & (1 << 1)) != 0)	kbdKey = 0x28;	// 'enter'
		// P3.0も使用できるがオンボードLEDが反応してしまうので未使用とする
		// if ((p3_lat & (1 << 0)) != 0)	kbdKey = 0xXX;	// ''
		if (kbdKey == 0x28) {
			OLED_P8x16Str(x, y, " ");
			x = 0;
			y += 2;
			OLED_P8x16Str(x, y, "_");
		} else {
			str[0] = 0x5D + kbdKey;
			str[1] = '\0';
			OLED_P8x16Str(x, y, str);
			OLED_P8x16Str(x + 8, y, "_");
			x += 8;
		}
		keyState = KBD_STATE_KEYDOWN;
    	USB_Keyboard_SendKey(0, kbdKey);
    	while(keyState == KBD_STATE_KEYDOWN);
		mDelaymS(10);	// キーを送信しきるために必要？
		keyState = KBD_STATE_KEYDOWN;
    	USB_Keyboard_SendKey(0, 0);
    	while(keyState == KBD_STATE_KEYDOWN);
		mDelaymS(10);	// キーを送信しきるために必要？
		mDelaymS(100);	// タクトスイッチの場合のバウンス時間
		while (1)
		{
			// 有効なポートのみを取得し、押下されたら1にする
			p1_lat = (~P1 & 0xF3);
			p3_lat = (~P3 & 0x06);
			// 全キーが押下されていなければ抜ける
			if ((p1_lat | p3_lat) == 0)	break;
		}
		mDelaymS(100);	// タクトスイッチの場合のバウンス時間
		LED = 0;
    }
}

/*
 * According to SDCC specification, interrupt handlers MUST be placed within the file which contains
 * the void main(void) function, otherwise SDCC won't be able to recognize it. It's not a bug but a feature.
 * If you know how to fix this, please let me know.
 */
void USBInterruptEntry(void) interrupt INT_NO_USB {
	USBInterrupt();
}

