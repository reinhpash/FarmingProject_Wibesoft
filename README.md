### Test Case

**Görev 1**: Ekin Sistemi
Basit bir grid tabanlı tarla üzerinde ekin ekme, büyütme ve hasat etme mekanizmasını geliştirin.
İçerik:
- En az iki farklı ekin türü ekleyin.
- Büyümeye özel animasyon veya değişen görseller kullanın.
- Oyuncunun ekin ekme ve hasat etme işlemlerini yönetmesini sağlayın.


**Görev 2**: Bina Yerleştirme Mekanizması
Grid tabanlı bir araziye binaların yerleştirilebileceği bir sistem oluşturun.
İçerik:
- Yerleştirme sırasında geçerli/geçersiz alanları görsel olarak belirtin.
- Oyuncunun binaları yeniden konumlandırabilmesini sağlayın.

## # Proje Genelinde Dikkat Ettiklerim
Projeyi geliştirirken olabildiğince modüler bir yapı inşaa etmeye çalıştım.

Bina yerleştirme sistemi scriptable object ile yazıldı ve yeni bina eklenmesi kolay ve modüler hale getirildi.

Aynı şekilde hasat sistemleri de scriptable objectler ile inşaa edildi bunun sayesinde oyuna çok daha fazla içeriği çok daha kısa sürede eklenebilir oldu.

Yazdığım kodların önemli kısımlarında genelde actionlar kullanıldı bunun sayesinde de projenin scale etmesi durumunda gerekeli olan esneklik sağlandı.

Hayday oyununda ki gibi bina ve ekin ekme işlemleri ui'dan drag and drop edilerek gerçekleştirildi.

## # Ekin Sistemi
Tüm ekin ekme, hasat etme gibi işlemler CropManager isimli managerdan gerçekleşmekte.
Ekinlerin temel sınıfı olan Harvest sınıfında ilgili hasatın datalarını kontrol etmek, tamamlandığını kontrol etmek ve hasat etme metodunu barındırıyor.
Sınıfın özellikleri sayesinde her hasat için özel büyüme süreleri, efektleri, büyüme efektleri ayarlayabiliyoruz.
Aşağıdaki gifte sağ tarafta hasatların büyüme ve hasat edilme anları bulunuyor.


![2025-03-0116-53-40-ezgif com-optimize](https://github.com/user-attachments/assets/da3cd6a1-37f1-407d-8fcc-381a9ca16b8f)


## # Bina yerleştirme sistemi
Bina yerleştirme sistemi grid tabanlı bir yapı ile geliştirildi.
Binaları yerleştirirken yerleşemeyeceği bir alanda ise objenin rengi değişecek şekilde yazıldı.
Scriptable Objectler kullanarak geliştirildiği için yeni binalar ekleme ve daha fazla içerik ekleme işlemi kolaylaşmış oldu.
![](https://i.imgur.com/V9tRi4e.png)

![](https://i.imgur.com/l9ooCA6.gif)

