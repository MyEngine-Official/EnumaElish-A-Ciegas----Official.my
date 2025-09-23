// apicpp.h
#pragma once

using namespace System;

namespace MiLibCppCli
{
    public ref class Matematicas
    {
    public:
        int Sumar(int a, int b)
        {
            return a + b;
        }

        String^ Saludar(String^ nombre)
        {
            return "Hola, " + nombre;
        }
    private:
    };
}
