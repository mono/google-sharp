if ENABLE_MONODOC
SOURCESDIR=$(libdir)/monodoc/sources
TARGETS=Mono.Google.tree Mono.Google.zip Mono.Google.source
UPDATER = $(MONODOCER) -path:Mono.Google $(UPDATE_ASSEMBLIES)
else
SOURCESDIR=$(datadir)
TARGETS=
UPDATER = echo "Documentation build is disabled."
endif

NAMESPACES=Mono.Google Mono.Google.Picasa
UPDATE_ASSEMBLIES = -assembly:$(top_builddir)/src/Mono.Google.dll

docsdir=$(SOURCESDIR)
docs_DATA=$(TARGETS)

CLEANFILES=Mono.Google.tree Mono.Google.zip
EXTRA_DIST=Mono.Google.source

Mono.Google: Mono.Google.tree

Mono.Google.tree Mono.Google.zip: $(srcdir)/Mono.Google/*.xml $(srcdir)/Mono.Google/*/*.xml
	mdassembler --ecma $(srcdir)/Mono.Google -o Mono.Google

install-hook:
	cp Mono.Google.* `pkg-config monodoc --variable=sourcesdir`

uninstall-hook:
	rm -f `pkg-config monodoc --variable=sourcesdir`/Mono.Google.*

update: 
	$(UPDATER)

update-delete: 
	$(UPDATER) --delete

merge:
	monodoc --merge-changes ~/.config/monodoc/changeset.xml

dist-hook:
	mkdir -p $(distdir)/Mono.Google
	cp $(srcdir)/Mono.Google/*.xml $(distdir)/Mono.Google/
	for i in $(NAMESPACES); do				\
		mkdir -p $(distdir)/Mono.Google/$$i;			\
		cp $(srcdir)/Mono.Google/$$i/*.xml $(distdir)/Mono.Google/$$i;	\
	done

