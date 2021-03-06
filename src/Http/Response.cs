﻿// Correios.Net - Biblioteca de comunicação com o site dos correios.
// Copyright (C) 2013  Lucas Andrade de Araújo <lucasdearaujo@icloud.com>

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Correios.Net.Http
{
    class Response
    {
        /// <summary>
        /// Texto de resposta recebido do servidor.
        /// </summary>
        public String Text { get; set; }

        /// <summary>
        ///  Construtor
        /// </summary>
        /// <param name="responseText">
        /// HTML retornado pelo servidor que
        /// será usado para construir o endereço
        /// </param>
        public Response(string responseText)
        {
            this.Text = responseText;
        }

        /// <summary>
        /// Método responsável por capturar um componente do endereço
        /// na responsta do servidor. Por Macelo de Souza.
        /// </summary>
        /// 
        /// <param name="tag"></param>
        /// <returns></returns>
        private string GetValueByTag(string tag)
        {
            try
            {
                string value = this.Text;

                value = value.Replace("\r", "").Replace("\n", "");
                value = value.Substring(value.IndexOf("<span class=\"resposta\">" + tag));
                value = value.Substring(value.IndexOf("<span class=\"respostadestaque\">") + 31);

                string result = value.Substring(0, value.IndexOf("</span>"));
                return result.Trim();
            }
            catch (ArgumentOutOfRangeException)
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Método responsável por realizar a conversão
        /// da resposta recebida do servidor para um objeto
        /// do tipo Address.
        /// </summary>
        /// <returns></returns>
        public Address ToAddress()
        {
            Address address = new Address();

            address.Street = this.GetValueByTag("Logradouro");
            address.District = this.GetValueByTag("Bairro");        
            address.City = Regex.Match(this.GetValueByTag("Localidade"), "^(.*?)   ").Groups[1].Value;
            address.State = Regex.Match(this.GetValueByTag("Localidade"), "([A-Z]{2})$").Groups[1].Value;
            address.Cep = this.GetValueByTag("CEP");
            
            if(address.Street == String.Empty)
                address.CepUnico = true;

            return address;
        }
    }
}
