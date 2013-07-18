all:
	find . -iname "*.cs" | xargs mono-csc -out:fasm.exe

clean:
	rm -f ./fasm.exe
