using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using BDMPro.Resources;

namespace BDMPro.CustomHelper
{
    public static class CustomHelper
    {
        public static IHtmlContent CustomDropDown(string inputName, List<SelectListItem> selectList, bool searchBar = true, string onChangeValueJsMethod = "", string placeholder = "", string selectedValue = "")
        {
            var div = new TagBuilder("div");
            div.AddCssClass("form-dropdown dropdown");
            div.GenerateId($"{inputName}-ddl", "_");

            var input = new TagBuilder("input");
            input.AddCssClass("dropdown-input");
            input.Attributes.Add("name", inputName);
            selectList = selectList == null ? new List<SelectListItem>() : selectList;
            selectedValue = (string.IsNullOrEmpty(selectedValue) ?
                selectList.FirstOrDefault(a => a?.Selected == true)?.Value : selectedValue ) ?? "";
            if (!string.IsNullOrEmpty(selectedValue))
            {
                input.Attributes.Add("value", selectedValue);
            }
            input.Attributes.Add("hidden", "");

            var button = new TagBuilder("button");
            button.AddCssClass("btn dropdown-toggle w-100 text-start border-1 text-muted");
            button.Attributes.Add("type", "button");
            button.Attributes.Add("data-bs-toggle", "dropdown");
            button.Attributes.Add("aria-expanded", "false");

            selectList = selectList == null ? new List<SelectListItem>() : selectList;
            string selectedText = (string.IsNullOrEmpty(selectedValue) ?
                selectList.FirstOrDefault(a => a?.Selected == true)?.Text :
                selectList.FirstOrDefault(a => a?.Value == selectedValue)?.Text) ?? "";
            string placeholderText = string.IsNullOrEmpty(placeholder) ?
                Resource.PleaseSelect : placeholder;
            string appendText = string.IsNullOrEmpty(selectedText) ? placeholderText : selectedText;
            button.Attributes.Add("placeholder", appendText);
            button.InnerHtml.Append(appendText);

            var ul = new TagBuilder("ul");
            ul.AddCssClass("dropdown-menu w-100");

            if (searchBar)
            {
                var searchLi = new TagBuilder("li");

                var inputGroup = new TagBuilder("div");
                inputGroup.AddCssClass("input-group");

                var inputGroupIcon = new TagBuilder("span");
                inputGroupIcon.AddCssClass("input-group-text bg-light");
                inputGroupIcon.InnerHtml.AppendHtml("<i class=\"fa-solid fa-magnifying-glass\"></i>");

                var searchInput = new TagBuilder("input");
                searchInput.AddCssClass("form-control search");
                searchInput.Attributes.Add("type", "text");
                searchInput.Attributes.Add("placeholder", Resource.SearchDot);
                searchInput.Attributes.Add("aria-label", "Search");

                inputGroup.InnerHtml.AppendHtml(inputGroupIcon);
                inputGroup.InnerHtml.AppendHtml(searchInput);

                searchLi.InnerHtml.AppendHtml(inputGroup);

                ul.InnerHtml.AppendHtml(searchLi);
            }

            if (selectList != null)
            {
                foreach (var item in selectList)
                {
                    var li = new TagBuilder("li");
                    li.AddCssClass("dropdown-list");
                    var a = new TagBuilder("a");
                    a.Attributes.Add("data-value", item.Value);
                    a.AddCssClass((selectedValue != null && selectedValue == item.Value) || item.Selected == true ? "dropdown-item active" : "dropdown-item");
                    if (!string.IsNullOrEmpty(onChangeValueJsMethod))
                    {
                        a.Attributes.Add("onclick", onChangeValueJsMethod + "(this)");
                    }

                    a.InnerHtml.Append(item.Text);
                    li.InnerHtml.AppendHtml(a);
                    ul.InnerHtml.AppendHtml(li);
                }
            }

            div.InnerHtml.AppendHtml(input);
            div.InnerHtml.AppendHtml(button);
            div.InnerHtml.AppendHtml(ul);

            return div;
        }

        public static IHtmlContent CustomRadioButton(string name, string id, string val, string label, string selectedVal, string defaultVal)
        {
            var buildDiv = new TagBuilder("div");
            buildDiv.Attributes.Add("class", "form-check form-check-inline");

            var buildInput = new TagBuilder("input");
            buildInput.GenerateId(id, "_");
            buildInput.Attributes.Add("class", "form-check-input");
            buildInput.Attributes.Add("type", "radio");
            buildInput.Attributes.Add("name", name);
            buildInput.Attributes.Add("for", id);
            buildInput.Attributes.Add("value", val.ToString());
            if (!string.IsNullOrEmpty(selectedVal))
            {
                if (val == selectedVal)
                {
                    buildInput.Attributes.Add("checked", "checked");
                }
            }
            else
            {
                if (val == defaultVal)
                {
                    buildInput.Attributes.Add("checked", "checked");
                }
            }

            var buildLabel = new TagBuilder("label");
            buildLabel.Attributes.Add("class", "form-check-label");
            buildLabel.Attributes.Add("for", id);
            buildLabel.InnerHtml.Append(label);

            buildDiv.InnerHtml.AppendHtml(buildInput);
            buildDiv.InnerHtml.AppendHtml(buildLabel);
            return buildDiv;
        }

        public static IHtmlContent CustomMultiSelect(string id, List<SelectListItem> selectListItems)
        {
            var buildDiv = new TagBuilder("div");

            var buildSelect = new TagBuilder("select");
            buildSelect.Attributes.Add("id", id);
            buildSelect.Attributes.Add("name", id);
            buildSelect.Attributes.Add("for", id);
            buildSelect.Attributes.Add("class", "multichoice");
            buildSelect.Attributes.Add("multiple", "multiple");
            buildSelect.Attributes.Add("placeholder", Resource.Selectmultiple);

            foreach (var item in selectListItems)
            {
                var buildOption = new TagBuilder("option");
                buildOption.Attributes.Add("value", item.Value);
                buildOption.InnerHtml.SetContent(item.Text);
                if (item.Selected)
                {
                    buildOption.Attributes.Add("selected", "selected");
                }

                buildSelect.InnerHtml.AppendHtml(buildOption);
            }

            buildDiv.InnerHtml.AppendHtml(buildSelect);

            return buildDiv;
        }

        public static Microsoft.AspNetCore.Html.HtmlString CustomMultiSelectFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var buildDiv = new TagBuilder("div");
            buildDiv.TagRenderMode = TagRenderMode.Normal;
            return new Microsoft.AspNetCore.Html.HtmlString(buildDiv.ToString());
        }

        public static IHtmlContent CustomSearchAndSelect(string id, string datalistId, List<SelectListItem> selectListItems)
        {
            var buildDiv = new TagBuilder("div");

            var buildInput = new TagBuilder("input");
            buildInput.Attributes.Add("id", id);
            buildInput.Attributes.Add("name", id);
            buildInput.Attributes.Add("for", id);
            buildInput.Attributes.Add("class", "form-control");
            buildInput.Attributes.Add("list", datalistId);
            buildInput.Attributes.Add("placeholder", Resource.TypetoSearch);

            var buildSelect = new TagBuilder("datalist");
            buildSelect.Attributes.Add("id", datalistId);

            foreach (var item in selectListItems)
            {
                var buildOption = new TagBuilder("option");
                buildOption.Attributes.Add("value", item.Value);
                buildOption.InnerHtml.SetContent(item.Text);
                if (item.Selected)
                {
                    buildInput.Attributes.Add("value", item.Value);
                    buildOption.Attributes.Add("selected", "selected");
                }

                buildSelect.InnerHtml.AppendHtml(buildOption);
            }

            buildDiv.InnerHtml.AppendHtml(buildInput);
            buildDiv.InnerHtml.AppendHtml(buildSelect);

            return buildDiv;
        }
    }
}