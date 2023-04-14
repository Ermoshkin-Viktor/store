using Microsoft.AspNetCore.Http;
using Store.Web.Models;
using System.IO;
using System.Text;

namespace Store.Web
{
    public static class SessionExtensions
    {
        private const string key = "Cart";
        //Сохранение значений корзины в сессии
        public static void Set(this ISession session, Cart value)
        {
            if(value== null)            
                return;
            
            using (var stream = new MemoryStream())
            using(var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                writer.Write(value.Items.Count);//к-во элементов
                
                foreach(var item in value.Items)
                {
                    writer.Write(item.Key); //Id
                    writer.Write(item.Value);//к-во книг
                }

                writer.Write(value.Amount);//цена товаров
                //Передаем ключ key под которым это все хранить в виде массива байтов
                session.Set(key, stream.ToArray());
            }
        }

        //Чтение 
        public static bool TryGetCart(this ISession session, out Cart value)
        {
            //если есть такое значение с таким ключом и оно находится в буфере
            if(session.TryGetValue(key, out byte[] buffer))
            {
                using(var stream = new MemoryStream(buffer))
                using(var reader = new BinaryReader(stream, Encoding.UTF8, true))
                {
                    value = new Cart();

                    //получаем длину массива
                    var length = reader.ReadInt32();
                    for(int i = 0; i < length; i++)
                    {
                        //читаем пары: ключ-значение
                        var bookId = reader.ReadInt32();
                        var count = reader.ReadInt32();

                        //добавляем эту пару в наш словарь
                        value.Items.Add(bookId, count);
                    }

                    //добавляем цену(децимал-значение)
                    value.Amount = reader.ReadDecimal();
                    //выводим true
                    return true;
                }
            }
            //если нет то выводим false
            value = null; 
            return false;
        }
    }
}
