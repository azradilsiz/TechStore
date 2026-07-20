namespace TechStore.API.Resources
{
    public record ProductTranslation(string SourceName, string DisplayName, string Description);

    public static class ProductTranslations
    {
        public static readonly IReadOnlyList<ProductTranslation> Items =
        [
            new("iPhone 5s", "iPhone 5s", "Kompakt tasarımıyla öne çıkan iPhone 5s, günlük kullanım için temel akıllı telefon özelliklerini güvenilir bir şekilde sunar."),
            new("iPhone 6", "iPhone 6", "Geniş ekranı ve ince tasarımıyla iPhone 6, günlük iletişim ve multimedya ihtiyaçları için kullanışlı bir deneyim sunar."),
            new("iPhone 13 Pro", "iPhone 13 Pro", "Güçlü işlemcisi, gelişmiş kamera sistemi ve yüksek kaliteli ekranıyla iPhone 13 Pro, üst düzey bir akıllı telefon deneyimi sunar."),
            new("iPhone X", "iPhone X", "OLED ekranı, Face ID teknolojisi ve güçlü performansıyla iPhone X, modern tasarımı işlevsel özelliklerle bir araya getirir."),
            new("Oppo A57", "Oppo A57", "Şık tasarımı ve dengeli donanımıyla Oppo A57, günlük kullanım için performans ve uygun fiyatı bir arada sunar."),
            new("Oppo F19 Pro Plus", "Oppo F19 Pro Plus", "Gelişmiş kamera özellikleri ve güçlü performansıyla Oppo F19 Pro Plus, fotoğraf ve günlük kullanım odaklı bir deneyim sunar."),
            new("Oppo K1", "Oppo K1", "Şık tasarımı ve güvenilir performansıyla Oppo K1, farklı günlük kullanım ihtiyaçlarına uygun özellikler sunar."),
            new("Realme C35", "Realme C35", "Temel akıllı telefon ihtiyaçlarına odaklanan Realme C35, kullanıcı dostu yapısı ve uygun fiyatıyla günlük kullanım için ideal bir seçenektir."),
            new("Realme X", "Realme X", "Şık tasarımı, etkileyici ekranı ve dengeli kamera özellikleriyle Realme X, orta segmentte güçlü bir kullanıcı deneyimi sunar."),
            new("Realme XT", "Realme XT", "Gelişmiş kamera sensörleriyle donatılan Realme XT, yüksek kaliteli fotoğraf ve video çekmek isteyen kullanıcılar için güçlü özellikler sunar."),
            new("Samsung Galaxy S7", "Samsung Galaxy S7", "Yüksek çözünürlüklü ekranı, güçlü kamerası ve sağlam performansıyla Samsung Galaxy S7, günlük kullanımda güvenilir bir deneyim sunar."),
            new("Samsung Galaxy S8", "Samsung Galaxy S8", "Sonsuz Ekran tasarımı, gelişmiş kamera özellikleri ve güçlü donanımıyla Samsung Galaxy S8, etkileyici bir görsel deneyim sunar."),
            new("Samsung Galaxy S10", "Samsung Galaxy S10", "Dinamik AMOLED ekranı, çok yönlü kamera sistemi ve güçlü performansıyla Samsung Galaxy S10, gelişmiş akıllı telefon özellikleri sunar."),
            new("Vivo S1", "Vivo S1", "Canlı ekranı, kullanışlı kamera sistemi ve güvenilir performansıyla Vivo S1, tasarım ile işlevselliği bir araya getirir."),
            new("Vivo V9", "Vivo V9", "Şık tasarımı, çift kamera sistemi ve selfie odaklı özellikleriyle Vivo V9, fotoğraf çekmeyi seven kullanıcılar için geliştirilmiştir."),
            new("Vivo X21", "Vivo X21", "Ekrana entegre parmak izi sensörü, yüksek çözünürlüklü ekranı ve gelişmiş kameralarıyla Vivo X21, yenilikçi özellikler sunar."),
            new("Apple MacBook Pro 14 Inch Space Grey", "Apple MacBook Pro 14 İnç Uzay Grisi", "M1 Pro çipi, Retina ekranı ve ince tasarımıyla 14 inç MacBook Pro, yüksek performans gerektiren işler için güçlü bir kullanım deneyimi sunar."),
            new("Asus Zenbook Pro Dual Screen Laptop", "Asus Zenbook Pro Çift Ekranlı Laptop", "Çift ekranı ve yüksek performanslı donanımıyla Asus Zenbook Pro, üretkenlik ve yaratıcı çalışmalar için çok yönlü bir kullanım sunar."),
            new("Huawei Matebook X Pro", "Huawei MateBook X Pro", "İnce tasarımı ve yüksek çözünürlüklü dokunmatik ekranıyla Huawei MateBook X Pro, hareket halindeki kullanıcılar için premium bir deneyim sunar."),
            new("Lenovo Yoga 920", "Lenovo Yoga 920", "Esnek menteşesi sayesinde laptop veya tablet olarak kullanılabilen Lenovo Yoga 920, taşınabilirlik ve çok yönlülüğü bir araya getirir."),
            new("New DELL XPS 13 9300 Laptop", "Dell XPS 13 9300 Laptop", "Kompakt yapısı, ince çerçeveli InfinityEdge ekranı ve güçlü donanımıyla Dell XPS 13 9300, farklı çalışma ihtiyaçlarına uyum sağlar."),
            new("Amazon Echo Plus", "Amazon Echo Plus", "Alexa sesli asistan desteği, kaliteli sesi ve akıllı ev cihazlarını yönetme özelliğiyle Amazon Echo Plus, işlevsel bir akıllı hoparlördür."),
            new("Apple Airpods", "Apple AirPods", "Kolay eşleşme, kaliteli ses ve Siri desteği sunan Apple AirPods, günlük kullanımda kesintisiz bir kablosuz ses deneyimi sağlar."),
            new("Apple AirPods Max Silver", "Apple AirPods Max Gümüş", "Yüksek kaliteli ses, uyarlanabilir ekolayzır ve aktif gürültü engelleme özellikleriyle Apple AirPods Max, etkileyici bir dinleme deneyimi sunar."),
            new("Apple Airpower Wireless Charger", "Apple AirPower Kablosuz Şarj Cihazı", "Uyumlu Apple cihazlarını kablo kullanmadan kolayca şarj etmeyi sağlayan Apple AirPower, pratik bir şarj deneyimi sunar."),
            new("Apple HomePod Mini Cosmic Grey", "Apple HomePod Mini Uzay Grisi", "Kompakt tasarımı, güçlü sesi ve Apple ekosistemiyle uyumlu yapısıyla HomePod Mini, akıllı ev deneyimini destekler."),
            new("Apple iPhone Charger", "Apple iPhone Şarj Cihazı", "iPhone modellerini hızlı ve verimli şekilde şarj etmek için tasarlanan Apple şarj cihazı, güvenilir güç aktarımı sağlar."),
            new("Apple MagSafe Battery Pack", "Apple MagSafe Pil Paketi", "MagSafe uyumlu iPhone modellerine mıknatısla kolayca bağlanan taşınabilir pil paketi, ihtiyaç duyulduğunda ek kullanım süresi sağlar."),
            new("Apple Watch Series 4 Gold", "Apple Watch Series 4 Altın", "Kalp atış hızı ölçümü, aktivite takibi ve Retina ekranıyla Apple Watch Series 4, şık tasarımı akıllı saat özellikleriyle birleştirir."),
            new("Beats Flex Wireless Earphones", "Beats Flex Kablosuz Kulaklık", "Manyetik kulaklık uçları ve 12 saate varan pil ömrüyle Beats Flex, günlük kullanım için konforlu ve pratik bir ses deneyimi sunar."),
            new("iPhone 12 Silicone Case with MagSafe Plum", "iPhone 12 MagSafe Uyumlu Silikon Kılıf - Erik", "iPhone 12 için tasarlanan koruyucu silikon kılıf, MagSafe desteği sayesinde uyumlu aksesuarların kolayca takılmasını sağlar."),
            new("Monopod", "Monopod", "Dengeli ve ayarlanabilir çekimler için tasarlanan monopod; selfie, grup fotoğrafı ve video çekimlerinde pratik kullanım sağlar."),
            new("Selfie Lamp with iPhone", "iPhone Uyumlu Selfie Işığı", "Ayarlanabilir LED ışığıyla selfie ve görüntülü görüşmelerde daha dengeli aydınlatma sağlayan taşınabilir bir telefon aksesuarıdır."),
            new("Selfie Stick Monopod", "Selfie Çubuğu Monopod", "Uzatılabilir ve katlanabilir yapısıyla selfie ve grup fotoğraflarını kolayca çekmeyi sağlayan, telefon ve kameralarla uyumlu bir aksesuardır."),
            new("TV Studio Camera Pedestal", "TV Stüdyosu Kamera Pedestalı", "Stüdyo ortamında akıcı ve hassas kamera hareketleri sağlamak için tasarlanan profesyonel bir kamera destek sistemidir."),
            new("iPad Mini 2021 Starlight", "iPad Mini 2021 Yıldız Işığı", "Kompakt tasarımı, Retina ekranı ve güçlü işlemcisiyle iPad Mini 2021, taşınabilir ve yüksek performanslı bir tablet deneyimi sunar."),
            new("Samsung Galaxy Tab S8 Plus Grey", "Samsung Galaxy Tab S8 Plus Gri", "Geniş AMOLED ekranı, güçlü işlemcisi ve S Pen desteğiyle Galaxy Tab S8 Plus, üretkenlik ve eğlence için yüksek performans sunar."),
            new("Samsung Galaxy Tab White", "Samsung Galaxy Tab Beyaz", "Canlı ekranı, uzun pil ömrü ve çok yönlü özellikleriyle Samsung Galaxy Tab, günlük işler ve eğlence için kullanışlı bir tablet deneyimi sunar.")
        ];

        public static ProductTranslation? Find(string productName)
        {
            return Items.FirstOrDefault(product =>
                product.SourceName.Equals(productName, StringComparison.OrdinalIgnoreCase) ||
                product.DisplayName.Equals(productName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
