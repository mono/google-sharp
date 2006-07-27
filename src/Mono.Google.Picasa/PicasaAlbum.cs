//
// Mono.Google.Picasa.PicasaAlbum.cs:
//
// Authors:
//	Gonzalo Paniagua Javier (gonzalo@ximian.com)
//
// (C) Copyright 2006 Novell, Inc. (http://www.novell.com)
//

// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace Mono.Google.Picasa {
	public class PicasaAlbum {
		GoogleConnection conn;
		PicasaWeb picasa_web;
		string title;
		string description;
		string id;
		string rsslink;
		string link;
		AlbumAccess access = AlbumAccess.Public;
		int num_photos = -1;
		int num_photos_remaining = -1;

		internal PicasaAlbum (PicasaWeb pw)
		{
			this.conn = pw.Connection;
			this.picasa_web = pw;
		}

		internal static PicasaAlbum ParseAlbumInfo (PicasaWeb pw, XmlNode nodeitem, XmlNamespaceManager nsmgr)
		{
			PicasaAlbum album = new PicasaAlbum (pw);
			album.title = nodeitem.SelectSingleNode ("title").InnerText;
			album.description = nodeitem.SelectSingleNode ("description").InnerText;
			album.link = nodeitem.SelectSingleNode ("link").InnerText;
			XmlNode node = nodeitem.SelectSingleNode ("gphoto:id", nsmgr);
			if (node != null)
				album.id = node.InnerText;
			node = nodeitem.SelectSingleNode ("gphoto:access", nsmgr);
			if (node != null) {
				string acc = node.InnerText;
				album.access = (acc == "public") ? AlbumAccess.Public : AlbumAccess.Private;
			}
			node = nodeitem.SelectSingleNode ("gphoto:rsslink", nsmgr);
			if (node != null) 
				album.rsslink = node.InnerText;

			node = nodeitem.SelectSingleNode ("gphoto:numphotos", nsmgr);
			if (node != null)
				album.num_photos = (int) UInt32.Parse (node.InnerText);

			node = nodeitem.SelectSingleNode ("gphoto:numphotosremaining", nsmgr);
			if (node != null)
				album.num_photos_remaining = (int) UInt32.Parse (node.InnerText);
			return album;
		}

		public PicasaPictureCollection GetPictures ()
		{
			string received = conn.DownloadString (rsslink);
			XmlDocument doc = new XmlDocument ();
			doc.LoadXml (received);
			XmlNamespaceManager nsmgr = new XmlNamespaceManager (doc.NameTable);
			nsmgr.AddNamespace ("photo", "http://www.pheed.com/pheed/");
			nsmgr.AddNamespace ("media", "http://search.yahoo.com/mrss/");
			nsmgr.AddNamespace ("gphoto", "http://picasaweb.google.com/lh/picasaweb/");
			
			XmlNode channel = doc.SelectSingleNode ("/rss/channel");
			PicasaPictureCollection coll = new PicasaPictureCollection ();
			foreach (XmlNode item in channel.SelectNodes ("item")) {
				coll.Add (PicasaPicture.ParsePictureInfo (conn, this, item, nsmgr));
			}
			coll.SetReadOnly ();
			return coll;
		}

		static byte [] crlf = new byte [] { 13, 10 };

		static string op_upload = 
			"<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n" +
			"<rss version=\"2.0\" xmlns:gphoto=\"http://www.temp.com/\">\n" +
			" <channel>\n" +
			"  <gphoto:user>{0}</gphoto:user>\n" +
			"  <gphoto:id>{1}</gphoto:id>\n" +
			"  <gphoto:op>createAndAppendPhotoToAlbum</gphoto:op>\n" +
			"  <item>\n" +
			"   <title>{2}</title>\n" +
			"   <description/>\n" +
			"   <gphoto:multipart>{3}</gphoto:multipart>\n" +
			"   <gphoto:layout>0.000000</gphoto:layout>\n" +
			//"   <gphoto:checksum>17077b37</gphoto:checksum>\n" +
			"   <gphoto:client>picasa</gphoto:client>\n" +
			"  </item>\n" +
			" </channel>\n" +
			"</rss>";

		static string disp_pic = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n";

		public string UploadPicture (string title, Stream input)
		{
			if (title == null)
				throw new ArgumentNullException ("title");

			if (input == null)
				throw new ArgumentNullException ("input");

			if (!input.CanRead)
				throw new ArgumentException ("Cannot read from stream", "input");

			string url = API.GetPostURL ();
			string bound_str1 = "---------------------" + url.GetHashCode ().ToString ("X");
			string bound_head = "--" + bound_str1 + "\r\n";
			string bound_end = "--" + bound_str1 + "--\r\n";
			byte [] bound_end_bytes = Encoding.UTF8.GetBytes (bound_end);

			HttpWebRequest request = (HttpWebRequest) WebRequest.Create (url);
			request.Method = "POST";
			request.CookieContainer = Connection.Cookies;
			request.ContentType = "multipart/form-data; boundary=" + bound_str1;
			request.UserAgent = "User-Agent: Mozilla/5.0 (X11; U; Linux i686; en-US; rv:1.8.0.4) Gecko/20060527 SUSE/1.5.0.4-1.3 Firefox/1.5.0.4";
			string referer = String.Format ("http://picasaweb.google.com/lh/webUpload?uname={0}&aid={1}", User, UniqueID);
			request.Referer = referer;

			StringBuilder sb = new StringBuilder ();
			sb.Append (bound_head);
			sb.Append ("Content-Disposition: form-data; name=\"xml\"\r\nContent-Type: text/plain; charset=utf8\r\n\r\n");
			sb.AppendFormat (op_upload, Connection.User, UniqueID, title, title.GetHashCode ().ToString ());
			sb.Append ("\r\n");
			sb.Append (bound_head);
			sb.AppendFormat (disp_pic, title.GetHashCode ().ToString (), title);
			sb.Append ("Content-Type: application/octet-stream\r\n\r\n");

			Stream req_stream = request.GetRequestStream ();
			byte [] data = Encoding.UTF8.GetBytes (sb.ToString ());
			req_stream.Write (data, 0, data.Length);
			data = new byte [8192];
			int nread;
			while ((nread = input.Read (data, 0, data.Length)) > 0) {
				req_stream.Write (data, 0, nread);
			}

			req_stream.Write (crlf, 0, crlf.Length);
			req_stream.Write (bound_end_bytes, 0, bound_end_bytes.Length);
			req_stream.Close ();

			HttpWebResponse response = null;
			try {
				response = (HttpWebResponse) request.GetResponse ();
			} catch (WebException wexc) {
				response = (HttpWebResponse) wexc.Response;
				if (response == null)
					throw;
			}

			string received = null;
			using (Stream stream = response.GetResponseStream ()) {
				StreamReader sr = new StreamReader (stream, Encoding.UTF8);
				received = sr.ReadToEnd ();
			}
			response.Close ();
			XmlDocument doc = new XmlDocument ();
			doc.LoadXml (received);
			XmlNode node = doc.SelectSingleNode ("/response/result");
			if (node == null)
				throw new UploadPictureException ("Invalid response from server");

			if (node.InnerText != "success") {
				node = doc.SelectSingleNode ("/response/reason");
				if (node == null)
					throw new UploadPictureException ("Unknown reason");
					
				throw new UploadPictureException (node.InnerText);
			}
			return doc.SelectSingleNode ("/response/id").InnerText;
		}

		public void UploadPicture (string filename)
		{
			if (filename == null)
				throw new ArgumentNullException ("filename");

			string title = Path.GetFileName (filename);
			using (Stream stream = File.OpenRead (filename)) {
				UploadPicture (title, stream);
			}
		}

		public string Title {
			get { return title; }
		}

		public string Description {
			get { return description; }
		}

		public string Link {
			get { return link; }
		}

		public string UniqueID {
			get { return id; }
		}

		public string RssLink {
			get { return rsslink; }
		}

		public AlbumAccess Access {
			get { return access; }
		}

		public int PicturesCount {
			get { return num_photos; }
		}

		public int PicturesRemaining {
			get { return num_photos_remaining; }
		}

		public string User {
			get { return picasa_web.User; }
		}

		internal PicasaV1 API {
			get { return picasa_web.API; }
		}

		internal GoogleConnection Connection {
			get { return conn; }
		}
	}
}

