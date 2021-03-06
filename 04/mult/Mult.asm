// This file is part of www.nand2tetris.org
// and the book "The Elements of Computing Systems"
// by Nisan and Schocken, MIT Press.
// File name: projects/04/Mult.asm

// Multiplies R0 and R1 and stores the result in R2.
// (R0, R1, R2 refer to RAM[0], RAM[1], and RAM[2], respectively.)

// Put your code here.

//REPASO
//Inicializar R2 = 0
@0
D=A
@R2
M=D

//Inicializar i = 16 y M[16] = 0
@i
M=0

//Los valores a multiplicar en R0 y R1 están ya metidos a mano
(LOOP)
@i
D=M
@R1
D=D-M
@END
D;JGE
@R2
D=M
@R0
D=D+M
@R2
M=D
@i
M=M+1
@LOOP
0;JMP

(END)
@END
0;JMP