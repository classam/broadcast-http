using System;

namespace broadcast.Concurrency
{
    public interface IStream<in T, out T2>
    {
        void In(T input);

        void ErrIn(Exception error);

        void LogIn(String log);
        
        T2 Out();

        Exception ErrOut();

        string LogOut();

        bool IsEmpty();

        bool ErrIsEmpty();

        bool LogIsEmpty();
        
        bool IsFull();

        bool ErrIsFull();

        bool LogIsFull();
    }
}