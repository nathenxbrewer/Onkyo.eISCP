using System.Collections.Generic;
using System.Xml.Serialization;

namespace Onkyo.eISCP.Model
{
// using System.Xml.Serialization;
// XmlSerializer serializer = new XmlSerializer(typeof(Device));
// using (StringReader reader = new StringReader(xml))
// {
//    var test = (Device)serializer.Deserialize(reader);
// }

[XmlRoot("response")]
public class Response
{
	[XmlAttribute("status")]
	public string Status { get; set; }

	[XmlElement("device")]
	public ReceiverInformation Device { get; set; }
}

[XmlRoot(ElementName="netservice")]
public class Netservice { 

	[XmlAttribute(AttributeName="id")] 
	public string Id { get; set; } 

	[XmlAttribute(AttributeName="value")] 
	public int Value { get; set; } 

	[XmlAttribute(AttributeName="name")] 
	public string Name { get; set; } 

	[XmlAttribute(AttributeName="account")] 
	public string Account { get; set; } 

	[XmlAttribute(AttributeName="password")] 
	public string Password { get; set; } 

	[XmlAttribute(AttributeName="zone")] 
	public int Zone { get; set; } 

	[XmlAttribute(AttributeName="enable")] 
	public int Enable { get; set; } 

	[XmlAttribute(AttributeName="multipage")] 
	public int Multipage { get; set; } 

	[XmlAttribute(AttributeName="addqueue")] 
	public int Addqueue { get; set; } 

	[XmlAttribute(AttributeName="sort")] 
	public int Sort { get; set; } 
}

[XmlRoot(ElementName="netservicelist")]
public class Netservicelist { 

	[XmlElement(ElementName="netservice")] 
	public List<Netservice> Netservice { get; set; } 

	[XmlAttribute(AttributeName="count")] 
	public int Count { get; set; } 
}

[XmlRoot(ElementName="zone")]
public class Zone { 

	[XmlAttribute(AttributeName="id")] 
	public int Id { get; set; } 

	[XmlAttribute(AttributeName="value")] 
	public int Value { get; set; } 

	[XmlAttribute(AttributeName="name")] 
	public string Name { get; set; } 

	[XmlAttribute(AttributeName="volmax")] 
	public int Volmax { get; set; } 

	[XmlAttribute(AttributeName="volstep")] 
	public int Volstep { get; set; } 

	[XmlAttribute(AttributeName="src")] 
	public int Src { get; set; } 

	[XmlAttribute(AttributeName="dst")] 
	public int Dst { get; set; } 

	[XmlAttribute(AttributeName="lrselect")] 
	public int Lrselect { get; set; } 
}

[XmlRoot(ElementName="zonelist")]
public class Zonelist { 

	[XmlElement(ElementName="zone")] 
	public List<Zone> Zone { get; set; } 

	[XmlAttribute(AttributeName="count")] 
	public int Count { get; set; } 
}

[XmlRoot(ElementName="selector")]
public class Selector { 

	[XmlAttribute(AttributeName="id")] 
	public string Id { get; set; } 

	[XmlAttribute(AttributeName="value")] 
	public int Value { get; set; } 

	[XmlAttribute(AttributeName="name")] 
	public string Name { get; set; } 

	[XmlAttribute(AttributeName="zone")] 
	public int Zone { get; set; } 

	[XmlAttribute(AttributeName="iconid")] 
	public string Iconid { get; set; } 
}

[XmlRoot(ElementName="selectorlist")]
public class Selectorlist { 

	[XmlElement(ElementName="selector")] 
	public List<Selector> Selector { get; set; } 

	[XmlAttribute(AttributeName="count")] 
	public int Count { get; set; } 
}

[XmlRoot(ElementName="preset")]
public class Preset { 

	[XmlAttribute(AttributeName="id")] 
	public string Id { get; set; } 

	[XmlAttribute(AttributeName="band")] 
	public int Band { get; set; } 

	[XmlAttribute(AttributeName="freq")] 
	public int Freq { get; set; } 

	[XmlAttribute(AttributeName="name")] 
	public string Name { get; set; } 
}

[XmlRoot(ElementName="presetlist")]
public class Presetlist { 

	[XmlElement(ElementName="preset")] 
	public List<Preset> Preset { get; set; } 

	[XmlAttribute(AttributeName="count")] 
	public int Count { get; set; } 
}

[XmlRoot(ElementName="control")]
public class Control { 

	[XmlAttribute(AttributeName="id")] 
	public string Id { get; set; } 

	[XmlAttribute(AttributeName="value")] 
	public int Value { get; set; } 

	[XmlAttribute(AttributeName="zone")] 
	public int Zone { get; set; } 

	[XmlAttribute(AttributeName="min")] 
	public int Min { get; set; } 

	[XmlAttribute(AttributeName="max")] 
	public int Max { get; set; } 

	[XmlAttribute(AttributeName="step")] 
	public int Step { get; set; } 

	[XmlAttribute(AttributeName="code")] 
	public string Code { get; set; } 

	[XmlAttribute(AttributeName="position")] 
	public int Position { get; set; } 

	[XmlAttribute(AttributeName="port")] 
	public int Port { get; set; } 
}

[XmlRoot(ElementName="controllist")]
public class Controllist { 

	[XmlElement(ElementName="control")] 
	public List<Control> Control { get; set; } 

	[XmlAttribute(AttributeName="count")] 
	public int Count { get; set; } 
}

[XmlRoot(ElementName="function")]
public class Function { 

	[XmlAttribute(AttributeName="id")] 
	public string Id { get; set; } 

	[XmlAttribute(AttributeName="value")] 
	public int Value { get; set; } 
}

[XmlRoot(ElementName="functionlist")]
public class Functionlist { 

	[XmlElement(ElementName="function")] 
	public List<Function> Function { get; set; } 

	[XmlAttribute(AttributeName="count")] 
	public int Count { get; set; } 
}

[XmlRoot(ElementName="tuner")]
public class Tuner { 

	[XmlAttribute(AttributeName="band")] 
	public string Band { get; set; } 

	[XmlAttribute(AttributeName="min")] 
	public int Min { get; set; } 

	[XmlAttribute(AttributeName="max")] 
	public int Max { get; set; } 

	[XmlAttribute(AttributeName="step")] 
	public int Step { get; set; } 
}

[XmlRoot(ElementName="tuners")]
public class Tuners { 

	[XmlElement(ElementName="tuner")] 
	public List<Tuner> Tuner { get; set; } 

	[XmlAttribute(AttributeName="count")] 
	public int Count { get; set; } 
}

[XmlRoot(ElementName="device")]
public class ReceiverInformation { 

	[XmlElement(ElementName="brand")] 
	public string Brand { get; set; } 

	[XmlElement(ElementName="category")] 
	public string Category { get; set; } 

	[XmlElement(ElementName="year")] 
	public int Year { get; set; } 

	[XmlElement(ElementName="model")] 
	public string Model { get; set; } 

	[XmlElement(ElementName="destination")] 
	public string Destination { get; set; } 

	[XmlElement(ElementName="productid")] 
	public string Productid { get; set; } 

	[XmlElement(ElementName="deviceserial")] 
	public string Deviceserial { get; set; } 

	[XmlElement(ElementName="macaddress")] 
	public string Macaddress { get; set; } 

	[XmlElement(ElementName="modeliconurl")] 
	public string Modeliconurl { get; set; } 

	[XmlElement(ElementName="friendlyname")] 
	public string Friendlyname { get; set; } 

	[XmlElement(ElementName="firmwareversion")] 
	public string Firmwareversion { get; set; } 

	[XmlElement(ElementName="ecosystemversion")] 
	public int Ecosystemversion { get; set; } 

	[XmlElement(ElementName="tidaloauthversion")] 
	public int Tidaloauthversion { get; set; } 

	[XmlElement(ElementName="netservicelist")] 
	public Netservicelist Netservicelist { get; set; } 

	[XmlElement(ElementName="zonelist")] 
	public Zonelist Zonelist { get; set; } 

	[XmlElement(ElementName="selectorlist")] 
	public Selectorlist Selectorlist { get; set; } 

	[XmlElement(ElementName="presetlist")] 
	public Presetlist Presetlist { get; set; } 

	[XmlElement(ElementName="controllist")] 
	public Controllist Controllist { get; set; } 

	[XmlElement(ElementName="functionlist")] 
	public Functionlist Functionlist { get; set; } 

	[XmlElement(ElementName="tuners")] 
	public Tuners Tuners { get; set; } 

	[XmlAttribute(AttributeName="id")] 
	public string Id { get; set; } 

	[XmlText] 
	public string Text { get; set; } 
}


}