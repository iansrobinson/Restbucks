﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Restbucks.MediaType.Assemblers
{
    public class FormsAssembler
    {
        private readonly XElement root;
        private readonly Uri parentBaseUri;

        public FormsAssembler(XElement root, Uri parentBaseUri)
        {
            this.root = root;
            this.parentBaseUri = parentBaseUri;
        }

        public IEnumerable<Form> AssembleForms()
        {
            var forms = root.DescendantsAndSelf(Namespaces.XForms + "model");
            return from form in forms
                   let submission = form.Element(Namespaces.XForms + "submission")
                   let instance = form.Element(Namespaces.XForms + "instance")
                   select new Form(
                       form.Attribute("id")
                           .ApplyNonEmptyValueOrDefault(),
                       submission.Attribute("resource")
                           .ApplyNonEmptyValueOrDefault(value => new Uri(value, UriKind.RelativeOrAbsolute)),
                       submission.Attribute("method")
                           .ApplyNonEmptyValueOrDefault(),
                       submission.Attribute("mediatype")
                           .ApplyNonEmptyValueOrDefault(),
                       form.Attribute("schema")
                           .ApplyNonEmptyValueOrDefault(value => new Uri(value, UriKind.RelativeOrAbsolute)),
                       instance
                           .ApplyNonNullInstance(inst => inst.Element(Namespaces.ShopSchema + "shop")
                                                             .ApplyNonNullInstance(shop => new ShopAssembler(shop, parentBaseUri).AssembleShop()))
                       );
        }
    }
}