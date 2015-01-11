

using UnityEngine;


//
// Autogenerated by gaxb ( https://github.com/SmallPlanet/gaxb )
//

using System;
using System.Xml;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;


public partial class PUInputField : PUInputFieldBase {
	
	public PUInputField()
	{
	}
	
	
	public PUInputField(
			string onValueChanged,
			string placeholder,
			int limit,
			PlanetUnity2.InputFieldContentType contentType,
			PlanetUnity2.InputFieldLineType lineType,
			Color selectionColor,
			string value ) : this()
	{
		this.onValueChanged = onValueChanged;
		this.onValueChangedExists = true;

		this.placeholder = placeholder;
		this.placeholderExists = true;

		this.limit = limit;
		this.limitExists = true;

		this.contentType = contentType;
		this.contentTypeExists = true;

		this.lineType = lineType;
		this.lineTypeExists = true;

		this.selectionColor = selectionColor;
		this.selectionColorExists = true;

		this.value = value;
		this.valueExists = true;
	}

	
	
	public PUInputField(
			string onValueChanged,
			string placeholder,
			int limit,
			PlanetUnity2.InputFieldContentType contentType,
			PlanetUnity2.InputFieldLineType lineType,
			Color selectionColor,
			string font,
			int fontSize,
			PlanetUnity2.FontStyle fontStyle,
			Color fontColor,
			float lineSpacing,
			PlanetUnity2.TextAlignment alignment,
			string value,
			bool bestFit,
			string onLinkClick,
			Vector4 bounds,
			Vector3 position,
			Vector2 size,
			Vector3 rotation,
			Vector3 scale,
			Vector2 pivot,
			string anchor,
			bool active,
			bool mask,
			Vector4 maskInset,
			bool outline,
			float lastY,
			float lastX,
			string shader,
			bool ignoreMouse,
			string components,
			string title,
			string tag,
			string tag1,
			string tag2,
			string tag3,
			string tag4,
			string tag5,
			string tag6 ) : this()
	{
		this.onValueChanged = onValueChanged;
		this.onValueChangedExists = true;

		this.placeholder = placeholder;
		this.placeholderExists = true;

		this.limit = limit;
		this.limitExists = true;

		this.contentType = contentType;
		this.contentTypeExists = true;

		this.lineType = lineType;
		this.lineTypeExists = true;

		this.selectionColor = selectionColor;
		this.selectionColorExists = true;

		this.font = font;
		this.fontExists = true;

		this.fontSize = fontSize;
		this.fontSizeExists = true;

		this.fontStyle = fontStyle;
		this.fontStyleExists = true;

		this.fontColor = fontColor;
		this.fontColorExists = true;

		this.lineSpacing = lineSpacing;
		this.lineSpacingExists = true;

		this.alignment = alignment;
		this.alignmentExists = true;

		this.value = value;
		this.valueExists = true;

		this.bestFit = bestFit;
		this.bestFitExists = true;

		this.onLinkClick = onLinkClick;
		this.onLinkClickExists = true;

		this.bounds = bounds;
		this.boundsExists = true;

		this.position = position;
		this.positionExists = true;

		this.size = size;
		this.sizeExists = true;

		this.rotation = rotation;
		this.rotationExists = true;

		this.scale = scale;
		this.scaleExists = true;

		this.pivot = pivot;
		this.pivotExists = true;

		this.anchor = anchor;
		this.anchorExists = true;

		this.active = active;
		this.activeExists = true;

		this.mask = mask;
		this.maskExists = true;

		this.maskInset = maskInset;
		this.maskInsetExists = true;

		this.outline = outline;
		this.outlineExists = true;

		this.lastY = lastY;
		this.lastYExists = true;

		this.lastX = lastX;
		this.lastXExists = true;

		this.shader = shader;
		this.shaderExists = true;

		this.ignoreMouse = ignoreMouse;
		this.ignoreMouseExists = true;

		this.components = components;
		this.componentsExists = true;

		this.title = title;
		this.titleExists = true;

		this.tag = tag;
		this.tagExists = true;

		this.tag1 = tag1;
		this.tag1Exists = true;

		this.tag2 = tag2;
		this.tag2Exists = true;

		this.tag3 = tag3;
		this.tag3Exists = true;

		this.tag4 = tag4;
		this.tag4Exists = true;

		this.tag5 = tag5;
		this.tag5Exists = true;

		this.tag6 = tag6;
		this.tag6Exists = true;
	}


}




public class PUInputFieldBase : PUText {


	private static Type planetOverride = Type.GetType("PlanetUnityOverride");
	private static MethodInfo processStringMethod = planetOverride.GetMethod("processString", BindingFlags.Public | BindingFlags.Static);




	// XML Attributes
	public string onValueChanged;
	public bool onValueChangedExists;

	public string placeholder;
	public bool placeholderExists;

	public int limit;
	public bool limitExists;

	public PlanetUnity2.InputFieldContentType contentType;
	public bool contentTypeExists;

	public PlanetUnity2.InputFieldLineType lineType;
	public bool lineTypeExists;

	public Color selectionColor;
	public bool selectionColorExists;




	
	public void SetOnValueChanged(string v) { onValueChanged = v; onValueChangedExists = true; } 
	public void SetPlaceholder(string v) { placeholder = v; placeholderExists = true; } 
	public void SetLimit(int v) { limit = v; limitExists = true; } 
	public void SetContentType(PlanetUnity2.InputFieldContentType v) { contentType = v; contentTypeExists = true; } 
	public void SetLineType(PlanetUnity2.InputFieldLineType v) { lineType = v; lineTypeExists = true; } 
	public void SetSelectionColor(Color v) { selectionColor = v; selectionColorExists = true; } 


	public override void gaxb_unload()
	{
		base.gaxb_unload();

	}
	
	public void gaxb_addToParent()
	{
		if(parent != null)
		{
			FieldInfo parentField = parent.GetType().GetField("InputField");
			List<object> parentChildren = null;
			
			if(parentField != null)
			{
				parentField.SetValue(parent, this);
				
				parentField = parent.GetType().GetField("InputFieldExists");
				parentField.SetValue(parent, true);
			}
			else
			{
				parentField = parent.GetType().GetField("InputFields");
				
				if(parentField != null)
				{
					parentChildren = (List<object>)(parentField.GetValue(parent));
				}
				else
				{
					parentField = parent.GetType().GetField("Texts");
					if(parentField != null)
					{
						parentChildren = (List<object>)(parentField.GetValue(parent));
					}
				}
				if(parentChildren == null)
				{
					FieldInfo childrenField = parent.GetType().GetField("children");
					if(childrenField != null)
					{
						parentChildren = (List<object>)childrenField.GetValue(parent);
					}
				}
				if(parentChildren != null)
				{
					parentChildren.Add(this);
				}
				
			}
		}
	}

	public override void gaxb_load(XmlReader reader, object _parent, Hashtable args)
	{
		base.gaxb_load(reader, _parent, args);

		if(reader == null && _parent == null)
			return;
		
		parent = _parent;
		
		if(this.GetType() == typeof( PUInputField ))
		{
			gaxb_addToParent();
		}
		
		xmlns = reader.GetAttribute("xmlns");
		

		string attr;
		attr = reader.GetAttribute("onValueChanged");
		if(attr != null && planetOverride != null) { attr = processStringMethod.Invoke(null, new [] {_parent, attr}).ToString(); }
		if(attr != null) { onValueChanged = attr; onValueChangedExists = true; } 
		
		attr = reader.GetAttribute("placeholder");
		if(attr != null && planetOverride != null) { attr = processStringMethod.Invoke(null, new [] {_parent, attr}).ToString(); }
		if(attr != null) { placeholder = attr; placeholderExists = true; } 
		
		attr = reader.GetAttribute("limit");
		if(attr != null && planetOverride != null) { attr = processStringMethod.Invoke(null, new [] {_parent, attr}).ToString(); }
		if(attr != null) { limit = int.Parse(attr); limitExists = true; } 
		
		attr = reader.GetAttribute("contentType");
		if(attr != null && planetOverride != null) { attr = processStringMethod.Invoke(null, new [] {_parent, attr}).ToString(); }
		if(attr != null) { contentType = (PlanetUnity2.InputFieldContentType)System.Enum.Parse(typeof(PlanetUnity2.InputFieldContentType), attr); contentTypeExists = true; } 
		
		attr = reader.GetAttribute("lineType");
		if(attr != null && planetOverride != null) { attr = processStringMethod.Invoke(null, new [] {_parent, attr}).ToString(); }
		if(attr != null) { lineType = (PlanetUnity2.InputFieldLineType)System.Enum.Parse(typeof(PlanetUnity2.InputFieldLineType), attr); lineTypeExists = true; } 
		
		attr = reader.GetAttribute("selectionColor");
		if(attr != null && planetOverride != null) { attr = processStringMethod.Invoke(null, new [] {_parent, attr}).ToString(); }
		if(attr != null) { selectionColor = new Color().PUParse(attr); selectionColorExists = true; } 
		

	}
	
	
	
	
	
	
	
	public override void gaxb_appendXMLAttributes(StringBuilder sb)
	{
		base.gaxb_appendXMLAttributes(sb);

		if(onValueChangedExists) { sb.AppendFormat (" {0}=\"{1}\"", "onValueChanged", onValueChanged); }
		if(placeholderExists) { sb.AppendFormat (" {0}=\"{1}\"", "placeholder", placeholder); }
		if(limitExists) { sb.AppendFormat (" {0}=\"{1}\"", "limit", limit); }
		if(contentTypeExists) { sb.AppendFormat (" {0}=\"{1}\"", "contentType", (int)contentType); }
		if(lineTypeExists) { sb.AppendFormat (" {0}=\"{1}\"", "lineType", (int)lineType); }
		if(selectionColorExists) { sb.AppendFormat (" {0}=\"{1}\"", "selectionColor", selectionColor); }

	}
	
	public override void gaxb_appendXMLSequences(StringBuilder sb)
	{
		base.gaxb_appendXMLSequences(sb);


	}
	
	public override void gaxb_appendXML(StringBuilder sb)
	{
		if(sb.Length == 0)
		{
			sb.AppendFormat ("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
		}
		
		sb.AppendFormat ("<{0}", "InputField");
		
		if(xmlns != null)
		{
			sb.AppendFormat (" {0}=\"{1}\"", "xmlns", xmlns);
		}
		
		gaxb_appendXMLAttributes(sb);
		
		
		StringBuilder seq = new StringBuilder();
		seq.AppendFormat(" ");
		gaxb_appendXMLSequences(seq);
		
		if(seq.Length == 1)
		{
			sb.AppendFormat (" />");
		}
		else
		{
			sb.AppendFormat (">{0}</{1}>", seq.ToString(), "InputField");
		}
	}
}
