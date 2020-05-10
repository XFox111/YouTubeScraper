using System;
using System.IO;
using System.Xml;

namespace YouTube.Models
{
	public class DashManifest
	{
		public string Label { get; }
		public DateTime ValidUntil { get; }
		public DashManifest(string label, XmlDocument doc)
		{
			Label = label;
			Xml = doc;
		}

		public string ManifestContent => Xml.OuterXml;
		public XmlDocument Xml { get; }
		public Uri WriteManifest(FileStream outStream)
		{
			Xml.Save(outStream);
			return new Uri(outStream.Name);
		}
	}
}