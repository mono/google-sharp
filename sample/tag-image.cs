//
// tag-image.cs
//
// Authors:
//	Stephane Delcroix (stephane@delcroix.org)
//
// (C) Copyright 2007 S. Delcroix
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
using System.Net;
using Mono.Google;
using Mono.Google.Picasa;

class Test {
	static void Main (string [] args)
	{
		string user = args [0];
		string albumid = args [1];
		string imageid = args [2];
		string tag = args [3];
		Console.Write ("Password: ");
		string password = Console.ReadLine ();
		if (password == null || password.Trim () == "")
			password = null;

		ServicePointManager.ServerCertificateValidationCallback +=  delegate { return true; };
		GoogleConnection conn = new GoogleConnection (GoogleService.Picasa);
		conn.Authenticate (user, password);
		PicasaPicture picture = new PicasaPicture (conn, albumid, imageid);
		Console.WriteLine ("  Image Name: {0} ID: {1}", picture.Title, picture.UniqueID);
		picture.AddTag (tag);
	}
}

