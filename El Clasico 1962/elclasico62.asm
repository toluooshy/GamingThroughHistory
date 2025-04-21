El Clasico 1962, written by Tolulope Oshinowo & Dwaha Daud, 04/2025

	ioh=iot i
	szm=sza sma-szf

define swpXY
	rcl 9s
	rcl 9s
	terminate

define drawPix YPOS, XPOS
	law XPOS
	add posY
	sal 8s
	swpXY

	law YPOS
	add posX
	sal 8s

	dpy-i 300
	ioh
	terminate

define drawBallCircle Y, X
	drawPix 0, 1
	drawPix 0, 3
	drawPix 4, 1
	drawPix 4, 3
	drawPix 1, 0
	drawPix 1, 3
	drawPix 1, 4
	drawPix 3, 4
	terminate

define drwRMpad RMX, RMY
	lac padW
	cma
	dac pad1Ctr
rmpLP,
	lac RMY
	add padW
	add pad1Ctr
	sal 8s
	swpXY

	lac RMX
	dpy-i 300
	ioh

	law 2
	add pad1Ctr
	dac pad1Ctr
	isp pad1Ctr

	jmp rmpLP+R
	terminate

define drwFCBpad FCBX, FCBY
	lac padW
	cma
	dac pad1Ctr
fcbLP,
	lac FCBY
	add padW
	add pad1Ctr
	sal 8s
	swpXY

	lac FCBX
	dpy-i 300
	ioh

	law 7
	add pad1Ctr
	dac pad1Ctr
	isp pad1Ctr

	jmp fcbLP+R
	terminate

define drawNet NLINE, NX
	law 0
	sub bottomScreen
	sub bottomScreen
	dac pad1Ctr
netLoop,
	lac pad1Ctr
	add bottomScreen
	sal 9s
	swpXY
	law NX
	dpy
	ioh

	law 25
	add pad1Ctr
	dac pad1Ctr
	isp pad1Ctr
	jmp netLoop+R
	terminate

/ Game main loop starts at 500/
500/
gameLoop,
	drawBallCircle
	lac posX
	add velX
	dac posX
	jsp checkX

	lac posY
	add velY
	dac posY
	jsp checkY

	drwRMpad leftX, rmY
	drwFCBpad rightX, fcbY

	jsp updatePads

	drawNet 0, 0
	jmp gameLoop

define keyTest KEY, JMPLOC
	lac keyState
	and KEY
	sza
	jmp JMPLOC
	terminate

define movePaddle PADY, MOVFUNC
	lac PADY
	dac padTempY
	jsp MOVFUNC
	lac padTempY
	dac PADY
	terminate

updatePads,
	dap retMove
	cli
	iot 11
	dio keyState

upRM,
	keyTest keyUpRM, upFCB
	movePaddle rmY, mvUp

upFCB,
	keyTest keyUpFCB, downRM
	movePaddle fcbY, mvUp

downRM,
	keyTest keyDownRM, downFCB
	movePaddle rmY, mvDown

downFCB,
	keyTest keyFCBDown, retMove
	movePaddle fcbY, mvDown

retMove,
	jmp .

define flipDir DIR
	lac DIR
	cma
	dac DIR
	terminate

mvUp,
	dap retUp
	lac padTempY
	sub padTopLimit
	sma
	jmp retUp
	lac padTempY
	add padOffset
	dac padTempY

	add randVal
	dac randVal
retUp,
	jmp .

mvDown,
	dap retDown
	lac padTempY
	add padBotLimit
	spa
	jmp retDown
	lac padTempY
	sub padOffset
	dac padTempY

	add randVal
	dac randVal
retDown,
	jmp .

doDelay,
	dap retDelay
	lac delaySet
	dac delayCnt
delayLoop,
	isp delayCnt
	jmp delayLoop
retDelay,
	jmp .

ballRestart,
	jsp doDelay
	idx resetCount

	lac randVal
	and dyMask
	add const1
	dac velY

	cla
	dac posX

	lac randVal
	and yMask
	sub bottomScreen
	dac posY

	lac resetCount
	and const1
	sza
	jmp rightRestart

leftRestart,
	law 2
	cma
	dac velX
	add screenEdge
	dac posX
	jmp retCheck

rightRestart,
	law 2
	dac velX
	sub screenEdge
	dac posX
	jmp retCheck

onPaddleHit,
	dap retCheck
	lac posY
	sub padTempY
	sub const1
	spa
	jmp ballRestart

	sub padW
	sma
	jmp ballRestart

	flipDir velX
	idx hitCtr

	lac velX
	spa
	jmp skipSpeedUp

	law 3
	sub hitCtr
	spa
	idx velX
	spa
	dzm hitCtr
skipSpeedUp,
	lac padTempY
	add padHalf
	sub posY
	spa
	cma
	sar 4s
	add const1

	lio velY
	spi
	cma

	dac velY
retCheck,
	jmp .

checkX,
	dap retCX
	lac rmY
	dac padTempY
	lac posX
	add rightScreen
	spa
	jsp onPaddleHit

	lac fcbY
	dac padTempY
	lac posX
	sub rightScreen
	sma
	jsp onPaddleHit
retCX,
	jmp .

checkY,
	dap retCY
	lac posY
	add bottomScreen
	spa
	jmp yCont
	flipDir velY

yCont,
	lac posY
	sub bottomScreen
	sma
	jmp retCY
	flipDir velY
retCY,
	jmp .

////////////////////////////////////////////////////////////////////////////////////////////////

/ Position and movement
posX,		000700
posY,		000000

velX,		777774
velY,		000003

resetCount,	000000

padOffset,	000004
randVal,	000001
padTempY,	000000

rmY,		000000
fcbY,		000000

pad1Ctr,	000000
keyState,	000000

leftX,		400400
rightX,		374000

padW,		000120
padHalf,	000064

const1,		000001

rightScreen, 000764
bottomScreen,   000764

screenEdge,	000500
dyMask,		000003
yMask,		000777

padTopLimit, 000562
padBotLimit, 000760

keyFCBDown,  000001
keyUpFCB,	000002

keyDownRM,	040000
keyUpRM, 	100000

delaySet,	770000
delayCnt,	000000
hitCtr,		000000

	start 500
