SUBDIRS=Mono.Google Mono.Google.Picasa

pkgconfigdir=$(libdir)/pkgconfig
MCSFLAGS= -debug+ -debug:full -nologo -r:System.Web.dll

monogoogledir = $(prefix)/lib/Mono.Google
pkgconfig_DATA = mono-google.pc
monogoogle_SCRIPTS = Mono.Google.dll

CLEANFILES = Mono.Google.dll*

# 
monogoogle_references=

monogoogle_sources  = \
			Mono.Google/Authentication.cs \
			Mono.Google/CaptchaException.cs \
			Mono.Google/GoogleService.cs \
			Mono.Google/GoogleConnection.cs \
			Mono.Google/MultipartRequest.cs \
			Mono.Google/NoCheckCertificatePolicy.cs \
			Mono.Google.Picasa/AlbumAccess.cs \
			Mono.Google.Picasa/CreateAlbumException.cs \
			Mono.Google.Picasa/DeleteAlbumException.cs \
			Mono.Google.Picasa/PicasaAlbum.cs \
			Mono.Google.Picasa/PicasaAlbumCollection.cs \
			Mono.Google.Picasa/PicasaPicture.cs \
			Mono.Google.Picasa/PicasaPictureCollection.cs \
			Mono.Google.Picasa/GDataApi.cs \
			Mono.Google.Picasa/PicasaWeb.cs \
			Mono.Google.Picasa/UploadPictureException.cs \
			Mono.Google.Picasa/UploadProgressEventArgs.cs \
			Mono.Google.Picasa/UploadProgressEventHandler.cs \
			Mono.Google.Picasa/XmlUtil.cs

monogoogle_build_sources = $(addprefix $(srcdir)/, $(monogoogle_sources)) $(addprefix $(top_builddir)/src/Mono.Google/, AssemblyInfo.cs)

Mono.Google.dll: $(monogoogle_build_sources)
	$(MCS) $(MCSFLAGS) $(monogoogle_references) /target:library /out:$@ $(monogoogle_build_sources)

