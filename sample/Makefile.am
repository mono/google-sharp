SAMPLES=list-albums.cs create-album.cs get-images.cs upload-image.cs
MCSFLAGS= -debug+ -debug:full -nologo -r:Mono.Google.dll

EXTRA_DIST=$(SAMPLES)
CLEANFILES= *.exe *.mdb *.pdb *.dll

noinst_SCRIPTS=$(SAMPLES:.cs=.exe)

%.exe: %.cs Mono.Google.dll
	$(MCS) $(MCSFLAGS) -out:$@ $<

Mono.Google.dll: $(top_builddir)/src/Mono.Google.dll
	cp $(top_builddir)/src/Mono.Google.dll* .

