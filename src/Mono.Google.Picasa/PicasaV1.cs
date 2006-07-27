//
// Mono.Google.Picasa.PicasaV1.cs:
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
using System.Xml;
namespace Mono.Google.Picasa {
	class PicasaV1 {
		static string url = "http://picasaweb.google.com/api/urls?version=1";

		//string photo_page;
		//string album_page;
		string gallery;
		//string gallery_page;
		string post;

		internal PicasaV1 (GoogleConnection conn)
		{
			string received = conn.DownloadString (url);
			// photoPage, albumPage, post, gallery and galleryPage
			XmlDocument doc = new XmlDocument ();
			doc.LoadXml (received);
			foreach (XmlNode node in doc.SelectNodes ("/rss/channel/item")) {
				XmlNode title_node = node.SelectSingleNode ("title");
				XmlNode link_node = node.SelectSingleNode ("link");
				string title = title_node.InnerText;
				switch (title) {
				case "gallery":
					gallery = link_node.InnerText;
					break;
				case "post":
					post = link_node.InnerText;
					break;
				default:
					break;
				}
			}
		}

		public string GetGalleryLink (string user)
		{
			return gallery.Replace ("{username}", user); // URLEncode user name?
		}

		public string GetPostURL ()
		{
			return post;
		}
	}
}

