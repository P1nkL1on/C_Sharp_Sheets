﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilat
{
    struct ASTFunctionCall : IOperation
    {

        int functionCallNumber;
        List<IOperation> arguments;

        public ASTFunctionCall(string s)
        {
            if (s.IndexOf("(") < 0)
                throw new Exception(s + " is not a function!");
            string approximateFuncName = s.Substring(0, s.IndexOf("("));
            bool foundAnalog = false; int i = 0;
            //define required types
            string incomeValuesString = MISC.getIn(s, s.IndexOf('('));

            List<ValueType> callingTypes;
            arguments = new List<IOperation>();

            if (incomeValuesString.Length > 0)
            {
                callingTypes = new List<ValueType>();
                List<string> incomeValues = MISC.splitBy(incomeValuesString, ',');
                for (int df = 0; df < incomeValues.Count; df++)
                {
                    arguments.Add(BinaryOperation.ParseFrom(incomeValues[df]));
                    callingTypes.Add(arguments[arguments.Count - 1].returnTypes());
                }
            }
            else
            {
                callingTypes = new List<ValueType>();
                callingTypes.Add(ValueType.Cvoid);
            }


            while (!foundAnalog && i < ASTTree.funcs.Count)
            {
                bool nameSame = (ASTTree.funcs[i].getName == approximateFuncName);
                // if same name then check correct of all types including
                if (nameSame)
                {
                    foundAnalog = true;
                    List<ValueType> requiredArgTypes = ASTTree.funcs[i].returnTypesList();
                    if (requiredArgTypes.Count == callingTypes.Count)
                    {
                        for (int j = 0; j < callingTypes.Count; j++)
                            if (callingTypes[j] != requiredArgTypes[j])
                                foundAnalog = false;    // не совпадает тип соответствующих аргументов
                    }
                    else
                        foundAnalog = false;    // не совпадает количество параметров
                }
                i++;
            }
            // declare
            functionCallNumber = i - 1;

            //make bug
            if (!foundAnalog)
                throw new Exception("Function with this name/arguments was never declared!");
        }

        public void Trace(int depth)
        {
            Console.WriteLine(String.Format("{0}{1}  #{3}[{2}]", MISC.tabs(depth), ASTTree.funcs[functionCallNumber].getName,
                              ASTTree.funcs[functionCallNumber].returnTypes().ToString(), functionCallNumber));
            for (int i = 0; i < arguments.Count; i++)
            {
                arguments[i].Trace(depth + 1);
                if (i == arguments.Count - 2)
                    MISC.finish = true;
            }
        }
        public ValueType returnTypes()
        {
            return ASTTree.funcs[functionCallNumber].returnTypes();
        }


    }
}