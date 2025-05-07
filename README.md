# 🎮 Gaming Through History: A Retro-Tech Journey Through Pong

**By Tolulope Oshinowo and Dwaha Daud**

> A time-traveling exploration of how hardware shaped games—reimagined through Pong.

---

## 📖 Overview

This repository contains **three era-specific implementations of Pong**, each developed using the tools and constraints of their respective periods. The goal: understand how programming languages, graphics systems, compilers, and input/output architectures have evolved over 60+ years—and how developers responded to those constraints creatively.

Each version is themed as **El Clásico**, the legendary soccer rivalry between **FC Barcelona** and **Real Madrid**.

Included in the repository:

- Fully playable builds for **Linux, Windows, and macOS**
- Source code and asset files for each game
- **Presentation slides** in `Gaming Through History.pdf`

---

## 📁 Repository Structure

```
GAMINGTHROUGHHISTORY/
├── El Clasico 1962/              # PDP-1 version (MiSTer-compatible)
│   └── elclasico62.rim           # Final binary to run
│   └── elclasico62.asm, .lst     # Source and listing file
│   └── macro1.c, macro1.exe      # PDP-1 assembler
│   └── cheatsheet.txt            # Notes on PDP-1 development
│
├── El Clasico 1985/              # NES version
│   └── elclasico85.nes           # Playable ROM
│   └── elclasico85.asm, .chr     # Code and custom sprite tiles
│   └── NESASM3.exe               # Assembler
│   └── nes-color-palette.png     # Sprite design aid
│
├── El Clasico Forever/           # Unity version
│   ├── Assets/, Library/, etc.   # Full Unity project structure
│   ├── Builds/                   # Executables by platform
│   │   ├── Windows/              # `El Clasico Forever.sfx.exe`
│   │   ├── MacOS/                # `El Clasico Forever.app`
│   │   └── Linux/                # `El Clasico Forever.AppImage`
│   └── Photon*.csproj            # Multiplayer support with Photon Engine
│
├── Gaming Through History.pdf    # 📊 Presentation slides
├── LICENSE
└── README.md                     # 👈 You are here
```

---

## 🕹️ How to Play

### 🖥️ El Clásico 1962 (PDP-1)

- **Platform**: MiSTer FPGA with PDP-1 core
- **To Run**:

  1. Copy `elclasico62.rim` to the SD card
  2. Load it in the PDP-1 core on your MiSTer device

- Input: Paddle movement via keyboard
- Notes: Limited graphical customization, but functional and era-authentic

---

### 🎮 El Clásico 1985 (NES)

- **Platform**: Any NES Emulator (e.g. [Mesen](https://www.mesen.ca/), [FCEUX](http://www.fceux.com/))
- **To Run**:

  1. Open `elclasico85.nes` in your emulator

- Features:

  - Custom `.chr` sprites for each team
  - Different paddle physics for FC Barcelona vs. Real Madrid

- Source includes:

  - `elclasico85.asm`: 6502 assembly source
  - `elclasico85.chr`: tile graphics
  - `NESASM3.exe`: compiler used

---

### 🕹️ El Clásico Forever (Unity)

- **Platform**: Windows, macOS, Linux
- **Online Multiplayer** via Photon Engine

#### ✅ To Play:

| OS          | File                                                    |
| ----------- | ------------------------------------------------------- |
| **Windows** | `Builds/Windows/El Clasico Forever.sfx.exe` (installer) |
| **macOS**   | `Builds/MacOS/El Clasico Forever.app` (native)          |
| **Linux**   | `Builds/Linux/El Clasico Forever.AppImage` (portable)   |

To run the AppImage on Linux:

```bash
chmod +x "El Clasico Forever.AppImage"
./"El Clasico Forever.AppImage"
```

#### Features:

- Full 3D rendering
- Stadium-style environment
- Custom paddle skins
- Multiplayer: Host or join via room codes
- Built using:

  - Unity C# Scripting
  - Photon Unity Networking (`PhotonRealtime`, `PhotonChat`, etc.)

---

## 💡 Learning Reflections

Our team gained insight into:

- Early graphics programming (PDP-1’s memory-mapped CRT plotting)
- The tradeoffs between 8-bit tile systems and modern 3D modeling
- How multiplayer is handled across networks and physics systems
- Tools from assembly compilers to online SDKs

See `Gaming Through History.pdf` for full reflection notes and slides.

---

## 📚 References & Resources

**PDP-1 Dev**

- [https://github.com/hrvach/fpg1](https://github.com/hrvach/fpg1)
- [https://www.youtube.com/watch?v=F5\_\_shDTYMQ](https://www.youtube.com/watch?v=F5__shDTYMQ)
- [https://github.com/simh/simtools/tree/master/crossassemblers/macro1](https://github.com/simh/simtools/tree/master/crossassemblers/macro1)

**NES Dev**

- [https://nerdy-nights.nes.science/](https://nerdy-nights.nes.science/)
- [https://skilldrick.github.io/easy6502/](https://skilldrick.github.io/easy6502/)
- [https://github.com/munshkr/nesasm](https://github.com/munshkr/nesasm)
- [https://www.romhacking.net/utilities/109/](https://www.romhacking.net/utilities/109/)
- [https://www.winehq.org/](https://www.winehq.org/)

**Unity / Photon**

- [https://unity.com](https://unity.com)
- [https://www.photonengine.com](https://www.photonengine.com)
