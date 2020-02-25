Console.WriteLine("漢字ドリルマシーン")
Console.WriteLine("漢字の書き取りの宿題ならおまかせ！")
Console.WriteLine("")

Console.WriteLine("書く漢字を入力してください")
v <- Console.ReadLine()
Console.WriteLine("回数を入力してください")
n <- Convert.ToNumber(Console.ReadLine())

Console.WriteLine(v + "を" + n +"回書きます")
Console.WriteLine("スタート")

for i <- 1 to n
  Console.WriteLine(v)
end

Console.WriteLine("終了!")