# paercebal.Gazō

Gazō is a simple tool used to keep images accessible to the clipboard, to help me in my usual workflow.

## Compilation

To build the binary, you'll need Visual Studio 2017/2019 Community Edition, with C#/WPF.

Open the Solution, launch the build, and that's all.

## Use case

When using the Windows Snipping Tool, you can only keep one image, and the Snipping
Tool itself is not (to my knowledge) multi-instance, which makes keeping temporary
images awkward.

In its basic version, Gazō can retrieve images from the clipboard, and keep them in
dedicated windows, where they remain available to be put again in the clipboard.
This enables me to manage multiple images without resorting in a heavier program like
GIMP.

In future versions, I expect I'll add load/save capabilities, as well as partial screen
captures (like the Snipping Tool, actually), with delays, etc..

# Features

- Capture selection from screen
- Controlled delay (from 0 to 5 seconds) before capturing screenshot
- Capture from Clipboard/Save into Clipboard
- Load from file/Save to file

# TODO

- Implement capture of hovered windows, instead of a selected square
- Implement capture of user-defined shape, instead of a selected square
- Make it more beautiful, because seriously, THIS IS SPARTA.
