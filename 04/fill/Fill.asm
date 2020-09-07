// This file is part of www.nand2tetris.org
// and the book "The Elements of Computing Systems"
// by Nisan and Schocken, MIT Press.
// File name: projects/04/Fill.asm

// Runs an infinite loop that listens to the keyboard input.
// When a key is pressed (any key), the program blackens the screen,
// i.e. writes "black" in every pixel;
// the screen should remain fully black as long as the key is pressed. 
// When no key is pressed, the program clears the screen, i.e. writes
// "white" in every pixel;
// the screen should remain fully clear as long as no key is pressed.

// Put your code here.

//PROBAR

@SCREEN
D=A
@addr 
M=D //M[16]=M[addr]=16384

(LOOP)
@KBD
D=M
@BLACK
D;JNE
@WHITE
D;JEQ
@LOOP
0;JMP

(BLACK)
@addr
A=M
M=-1
@24575
D=A
@addr
D=M-D
@LOOP
D;JGE
@addr
M=M+1 //en la última vuelta -> M[addr]=24575
@BLACK
0;JMP

(WHITE)
@addr
A=M
M=0
@16384
D=A
@addr //en la primera vuelta -> M[addr]=24575
D=D-M
@LOOP
D;JGE
@addr
M=M-1
@WHITE
0;JMP