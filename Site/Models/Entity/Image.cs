﻿using System;
using System.Globalization;
using System.Xml;
using Sdl.Web.Common.Configuration;
using Sdl.Web.Common.Interfaces;
using Sdl.Web.Common.Models;

namespace Sdl.Web.Modules.Core.Models
{
    [SemanticEntity(SchemaOrgVocabulary, "ImageObject", Prefix = "s", Public = true)]
    [SemanticEntity(CoreVocabulary, "Image", Prefix = "c")]
    [Serializable]
    public class Image : MediaItem
    {
        private string _altTxt;

        [SemanticProperty("s:name")]
        [SemanticProperty("c:altText")]
        public string AlternateText
        {
            // We do this so we can return an empty string if alt is not provided. This is due to the accessibility 
            // specs stating that an altText attribute MUST be present.
            get { return _altTxt ?? string.Empty; }
            set { _altTxt = value; }
        }

        /// <summary>
        /// Renders an HTML representation of the Media Item.
        /// </summary>
        /// <param name="widthFactor">The factor to apply to the width - can be % (eg "100%") or absolute (eg "120").</param>
        /// <param name="aspect">The aspect ratio to apply.</param>
        /// <param name="cssClass">Optional CSS class name(s) to apply.</param>
        /// <param name="containerSize">The size (in grid column units) of the containing element.</param>
        /// <returns>The HTML representation.</returns>
        /// <remarks>
        /// This method is used by the <see cref="IRichTextFragment.ToHtml()"/> implementation in <see cref="MediaItem"/> and by the HtmlHelperExtensions.Media implementation.
        /// Both cases should be avoided, since HTML rendering should be done in View code rather than in Model code.
        /// </remarks>
        public override string ToHtml(string widthFactor, double aspect = 0, string cssClass = null, int containerSize = 0)
        {
            if (string.IsNullOrEmpty(Url))
                return string.Empty;

            string responsiveImageUrl = SiteConfiguration.MediaHelper.GetResponsiveImageUrl(Url, aspect, widthFactor, containerSize);
            string dataAspect = (Math.Truncate(aspect * 100) / 100).ToString(CultureInfo.InvariantCulture);
            string widthAttr = string.IsNullOrEmpty(widthFactor) ? null : $"width=\"{widthFactor}\"";
            string classAttr = string.IsNullOrEmpty(cssClass) ? null : $"class=\"{cssClass}\"";
            return
                $"<img src=\"{responsiveImageUrl}\" alt=\"{AlternateText}\" data-aspect=\"{dataAspect}\" {widthAttr}{classAttr}/>";
        }

        /// <summary>
        /// Read additional properties from XHTML element.
        /// </summary>
        /// <param name="xhtmlElement">XHTML element</param>
        public override void ReadFromXhtmlElement(XmlElement xhtmlElement)
        {
            base.ReadFromXhtmlElement(xhtmlElement);
            AlternateText = xhtmlElement.GetAttribute("alt");
        }

        /// <summary>
        /// Gets the default View.
        /// </summary>
        /// <param name="localization">The context Localization</param>
        /// <remarks>
        /// This makes it possible possible to render "embedded" Image Models using the Html.DxaEntity method.
        /// </remarks>
        public override MvcData GetDefaultView(Localization localization)
        {
            return new MvcData("Core:Image");
        }
    }
}