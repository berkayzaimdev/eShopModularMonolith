# Seperating Schemas

- Projemizde "Seperate Schema" yaklaşımını uygulayacağız.
- Her modülün verisi aynı DB'de lakin farklı şemalar ile saklanacak.
- Bu yaklaşım sayesinde izolasyon ve yönetim kolaylığı açısından bir denge sağlayabileceğiz. Farklı DB'lerde olmasından daha kolay bir yönetim fakat daha az izole veri yığınları olacak.
- Verilerin mantıksal ayrımı sağlanacak, bu sayede bakım kolaylığı ve güvenlik de beraberinde gelecektir.
- Bu yaklaşımda; bir modülün şemasında yapılan değişiklik diğer modülleri etkilemeyecek.
- Pratik olarak açıklamak gerekirse; aynı DB'den her bir Entity için farklı context oluşturacağız
- ```modelBuilder.HasDefaultSchema("catalog");``` kodu burada etkili olacak

---

# Applying Keycloak

## Keycloak nedir?

- Keycloak; açık kaynak ve güçlü bir Authentication & Authorization provider'ıdır. OpenID ve OAuth2 teknolojilerini destekler.
	- OAuth2: Authorization için önemli bir teknolojidir. Resource Owner, Client, Authorization Server, Resource Server gibi terimlerle OpenID ilgilenir.
	- OpenID: OAuth2'le uyumlu bir yetkilendirme katmanıdır. JWT Token kullanır ve user bilgilerini bu token'da saklar. UserInfo endpoint ile kullanıcı hakkında detaylı bilgi sunar.


- SSO, Identity Brokering ve Social Login yapılarını sunmaktadır.
- SSO: Kullanıcılar tek bir oturum açma ile farklı uygulamalara erişebilir
- Identity Brokering ve Social Login: Google, Facebook, GitHub gibi dış identity provider'larla entegrasyon
- Best Practice şekilde kullanmak için;
	- docker-compose dosyası ile orkestre edilmeli
	- Fallback mekanizmasını ve sunucunun korunmasını sağlamak adına Kubernetes Node'ları ile çalışılmalı
	- DB olarak PostgreSQL tercih edilmelidir


## OAuth2 + OpenID Bağlantı Akışları

### Authorization Code Flow

1. Client, user'ı Keycloak giriş sayfasına yönlendirir.
1. User authenticate olur.
1. Keycloak, user'ı authentication kodu ile geri gönderir.
1. Client, bu kodu access token ve ID token ile değiştirir.

### Implicit Flow

- SPA'ler, token'ı direkt olarak döndürdüğü için bu yaklaşımı güvenli bir şekilde sağlayamadığından yeni uygulamalar için önerilmez.

### Client Credentials Flow

- M2M bağlantısında kullanılır. Client, doğrudan client ID'sini ve secret key'ini kullanarak Keycloak tarafından token'ını alabilir.

### Resource Owner Password Credentials Flow

- Kullanıcının kimlik bilgilerini toplayıp, bu bilgiler ile access token'ı elde etme
- Bu yöntem sadece güvenilir client'lar için kullanılmalıdır, bunun dışında ise önerilmez

### OpenID Connect Flow

- OAuth2 Authorization Code Flow'u bir ID token için extend etme
- Bu yöntem sadece user'ı doğrulayıp bu user'ın basit profil bilgisine erişmek için idealdir

## Keycloak'un Kurulumu

- docker-compose ve docker-compose-override dosyalarına gerekli eklemeler yapılır.
- DB'de

	```
	CREATE SCHEMA identity;
	GRANT ALL ON SCHEMA identity TO postgres;
	```
   komutları yürütülür.
- docker-compose projesi ayağa kaldırılır. Artık Keycloak'a ait verileri barındıran tablolar, seçtiğimiz schema'ya eklenmiştir. 
> (Çok sayıda hata alındı, komutlar pgadmin yerine container cli'de yürütülüp proje baştan başlatılınca tablolar ekleniyordu, lakin garip bir şekilde db duplicate olmuş durumdaydı. PostgreSQL'i silince ve Docker'da tüm container'ları volume'ları ile birlikte temizleyince düzeldi)

---

# Applying Outbox Pattern

