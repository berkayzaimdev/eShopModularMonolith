# Seperating Schemas

- Projemizde "Seperate Schema" yaklaşımını uygulayacağız.
- Her modülün verisi aynı DB'de lakin farklı şemalar ile saklanacak.
- Bu yaklaşım sayesinde izolasyon ve yönetim kolaylığı açısından bir denge sağlayabileceğiz. Farklı DB'lerde olmasından daha kolay bir yönetim fakat daha az izole veri yığınları olacak.
- Verilerin mantıksal ayrımı sağlanacak, bu sayede bakım kolaylığı ve güvenlik de beraberinde gelecektir.
- Bu yaklaşımda; bir modülün şemasında yapılan değişiklik diğer modülleri etkilemeyecek.
- Pratik olarak açıklamak gerekirse; aynı DB'den her bir Entity için farklı context oluşturacağız
- ```modelBuilder.HasDefaultSchema("catalog");``` kodu burada etkili olacak