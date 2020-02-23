msg <- [40, 41, 40 + 1 * 2][1 + 1] + "Hello" + "World!"
Console.WriteLine(msg)
begin
  msg <- "Hello block"
  Console.WriteLine(msg)
end
Console.WriteLine(1 + 1 == 2)
Console.WriteLine(1 < 2)
Console.WriteLine(not not true)

for i <- [1, 2, 3]
  Console.WriteLine(i)
end

if false
  Console.WriteLine("OK1")
elif 0 > 0
  Console.WriteLine("OK2")
else
  Console.WriteLine("OK3")
end