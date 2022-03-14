using System;

public class My_Locks
{

	private readonly object _thread_com_lock;
	private readonly object _thread_serveur_jeu_lock;

	public My_Locks()
	{
		_thread_com_lock = new object();
		_thread_serveur_jeu_lock = new object();
	}

	public object Get_thread_com_lock()
    {
		return _thread_com_lock;

	}

	public object Get_thread_serveur_jeu_lock()
	{
		return _thread_serveur_jeu_lock;

	}
}
