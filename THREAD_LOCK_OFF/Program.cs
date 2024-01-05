using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;

class Program
{
    public static ArrayList arrayList = new ArrayList();
    public static ArrayList oddarrayList = new ArrayList();
    public static ArrayList evenarrayList = new ArrayList();
    public static ArrayList primearrayList = new ArrayList();
    public static ArrayList[] dividedarrayLists;
    static ManualResetEvent even1 = new ManualResetEvent(false);
    static ManualResetEvent even2 = new ManualResetEvent(false);
    static ManualResetEvent even3 = new ManualResetEvent(false);
    static ManualResetEvent even4 = new ManualResetEvent(false);

    class ThreadParameters
    {
        public int INDEX { get; set; }
        public ManualResetEvent EventParameter { get; set; }

        public ThreadParameters(int index, ManualResetEvent eventParam)
        {
            INDEX = index;
            EventParameter = eventParam;
        }
    }
    static void Main(string[] args)
    {
        // Listeye 1.000.000 öğe ekle
        int i = 0;
        while (i < 1000000)
        {
            arrayList.Add(i);
            i++;
        }

        // ArrayList'teki öğeleri yazdır
        dividedarrayLists = DivideArrayList(arrayList, 4);
        Stopwatch stopwatch = new Stopwatch();

        stopwatch.Start();
        Console.WriteLine("Toplam çalışma süresi sayacı başladı.");

        Thread thread1 = new Thread(Function);
        Thread thread2 = new Thread(Function);
        Thread thread3 = new Thread(Function);
        Thread thread4 = new Thread(Function);

        ThreadParameters parameters1 = new ThreadParameters(0, even1);
        ThreadParameters parameters2 = new ThreadParameters(1, even2);
        ThreadParameters parameters3 = new ThreadParameters(2, even3);
        ThreadParameters parameters4 = new ThreadParameters(3, even4);


        thread1.Priority = ThreadPriority.Highest;
        thread1.Start(parameters1);

        thread2.Priority = ThreadPriority.AboveNormal;
        thread2.Start(parameters2);

        thread3.Priority = ThreadPriority.Normal;
        thread3.Start(parameters3);

        thread4.Priority = ThreadPriority.BelowNormal;
        thread4.Start(parameters4);

        // Tüm olayların tamamlanmasını bekleyin
        WaitHandle.WaitAll(new WaitHandle[] { even1, even2, even3, even4 });

        stopwatch.Stop();

        Console.WriteLine($"Toplam thread çalışma süresi: {stopwatch.ElapsedMilliseconds} milisaniye, " + $"{stopwatch.ElapsedMilliseconds / 1000.0} saniye.");
        Console.WriteLine("Asal sayı adedi : " + primearrayList.Count);
        Console.WriteLine("Tek sayı adedi: " + oddarrayList.Count);
        Console.WriteLine("Çift sayı adedi :" + evenarrayList.Count);
        Console.ReadLine();

    }

    static ArrayList[] DivideArrayList(ArrayList List, int parts)
    {
        if (List == null)
            throw new ArgumentNullException(nameof(List));

        if (parts <= 0)
            throw new ArgumentException("The number of parts must be greater than 0.", nameof(parts));

        int itemsPerPart = List.Count / parts;
        ArrayList[] dividedLists = new ArrayList[parts];

        int index = 0;
        for (int i = 0; i < parts; i++)
        {
            int size = (i < List.Count % parts) ? itemsPerPart + 1 : itemsPerPart;
            dividedLists[i] = new ArrayList(List.GetRange(index, size));
            index += size;
        }

        return dividedLists;
    }

    static void Function(object parameter)
    {
        ThreadParameters parameters = (ThreadParameters)parameter;
        ManualResetEvent eventParam = parameters.EventParameter;
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        for (int i = 0; i < dividedarrayLists[parameters.INDEX].Count; i++)
        {
            int count = 0;
            for (int j = 2; j <= (int)dividedarrayLists[parameters.INDEX][i] / 2; j++)
            {
                if ((int)dividedarrayLists[parameters.INDEX][i] % j == 0)
                {
                    count++;
                    break; // asal olmadığı anlaşılır anında çık
                }
            }

            if (count == 0)
            {
                if ((int)dividedarrayLists[parameters.INDEX][i] != 0 && (int)dividedarrayLists[parameters.INDEX][i] != 1)
                {
                    primearrayList.Add((int)dividedarrayLists[parameters.INDEX][i]);
                }
            }

            if ((int)dividedarrayLists[parameters.INDEX][i] % 2 == 0)
            {
                evenarrayList.Add(dividedarrayLists[parameters.INDEX][i]);
            }
            else
            {
                oddarrayList.Add(dividedarrayLists[parameters.INDEX][i]);
            }
        }
        stopwatch.Stop();
        Console.WriteLine("Thread " + (int)((int)parameters.INDEX + (int)1 )+ " çalışma süresi: " + stopwatch.ElapsedMilliseconds + " milisaniye, " + stopwatch.ElapsedMilliseconds / 1000.0 + " saniye.");
        eventParam.Set();
    }
}