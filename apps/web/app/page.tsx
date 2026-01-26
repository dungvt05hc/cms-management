import { getCategoryTree, getFeaturedProducts, type CategoryTreeNode, type Product } from "@/lib/api";
import HeroSlider from "@/components/home/HeroSlider";
import SectionTitle from "@/components/ui/SectionTitle";
import ProductCard from "@/components/product/ProductCard";
import CategoryCard from "@/components/category/CategoryCard";
import styles from "./page.module.css";

export default async function HomePage() {
  let categories: CategoryTreeNode[] = [];
  let featuredProducts: Product[] = [];
  let categoriesError: string | null = null;
  let productsError: string | null = null;

  try {
    categories = await getCategoryTree();
  } catch (err) {
    categoriesError = (err as Error).message;
    console.error("Failed to fetch categories:", err);
  }

  try {
    featuredProducts = await getFeaturedProducts(8);
  } catch (err) {
    productsError = (err as Error).message;
    console.error("Failed to fetch featured products:", err);
  }

  return (
    <>
      {/* Hero Slider */}
      <HeroSlider />

      {/* Categories Section */}
      <section className={styles.section}>
        <div className={styles.container}>
          <SectionTitle 
            title="Shop by Category" 
            subtitle="Browse our wide range of categories"
            viewAllLink="/products"
            viewAllText="View All Categories"
          />
          
          {categoriesError && (
            <div data-testid="category-error" className={styles.errorMessage}>
              Error loading categories: {categoriesError}
            </div>
          )}
          
          <div className={styles.categoriesGrid}>
            {categories.map((category) => (
              <CategoryCard 
                key={category.id}
                id={category.id}
                name={category.name}
              />
            ))}
            {categories.length === 0 && !categoriesError && (
              <>
                <CategoryCard id="1" name="Electronics" />
                <CategoryCard id="2" name="Fashion" />
                <CategoryCard id="3" name="Home & Living" />
                <CategoryCard id="4" name="Beauty" />
                <CategoryCard id="5" name="Sports" />
                <CategoryCard id="6" name="Books" />
              </>
            )}
          </div>
        </div>
      </section>

      {/* Promotional Banners */}
      <section className={styles.section}>
        <div className={styles.container}>
          <div className={styles.promoBanners}>
            <div className={styles.promoBanner} style={{ background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)' }}>
              <div className={styles.promoContent}>
                <span className={styles.promoTag}>Limited Time</span>
                <h3>Summer Sale</h3>
                <p>Up to 50% off on selected items</p>
                <a href="/products?sale=true" className={styles.promoButton}>Shop Now</a>
              </div>
              <div className={styles.promoImage}>🌞</div>
            </div>
            <div className={styles.promoBanner} style={{ background: 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)' }}>
              <div className={styles.promoContent}>
                <span className={styles.promoTag}>New Arrival</span>
                <h3>Trending Products</h3>
                <p>Check out the latest collection</p>
                <a href="/products?featured=true" className={styles.promoButton}>Explore</a>
              </div>
              <div className={styles.promoImage}>✨</div>
            </div>
          </div>
        </div>
      </section>

      {/* Featured Products Section */}
      <section className={styles.section}>
        <div className={styles.container}>
          <SectionTitle 
            title="Featured Products" 
            subtitle="Handpicked products just for you"
            viewAllLink="/products?featured=true"
            viewAllText="View All Products"
          />
          
          {productsError && (
            <div data-testid="products-error" className={styles.errorMessage}>
              Error loading products: {productsError}
            </div>
          )}
          
          <div className={styles.productsGrid}>
            {featuredProducts.map((product) => (
              <ProductCard key={product.id} product={product} />
            ))}
            {featuredProducts.length === 0 && !productsError && (
              <div className={styles.emptyState}>
                <p>No featured products available at the moment.</p>
                <a href="/products" className={styles.browseLink}>Browse All Products</a>
              </div>
            )}
          </div>
        </div>
      </section>

      {/* Why Choose Us Section */}
      <section className={styles.section} style={{ backgroundColor: 'var(--gray-50)' }}>
        <div className={styles.container}>
          <SectionTitle 
            title="Why Choose Us" 
            subtitle="We provide the best shopping experience"
            centered
          />
          
          <div className={styles.benefitsGrid}>
            <div className={styles.benefitCard}>
              <div className={styles.benefitIcon}>🚚</div>
              <h4>Free & Fast Delivery</h4>
              <p>Free shipping on all orders over $50. Express delivery available.</p>
            </div>
            <div className={styles.benefitCard}>
              <div className={styles.benefitIcon}>💯</div>
              <h4>Quality Guarantee</h4>
              <p>All products are quality checked before shipping to you.</p>
            </div>
            <div className={styles.benefitCard}>
              <div className={styles.benefitIcon}>🔄</div>
              <h4>Easy Returns</h4>
              <p>30-day easy return policy for a hassle-free experience.</p>
            </div>
            <div className={styles.benefitCard}>
              <div className={styles.benefitIcon}>🛡️</div>
              <h4>Secure Payment</h4>
              <p>Multiple secure payment options for your convenience.</p>
            </div>
          </div>
        </div>
      </section>

      {/* Newsletter Section */}
      <section className={styles.newsletterSection}>
        <div className={styles.container}>
          <div className={styles.newsletterContent}>
            <h2>Subscribe to Our Newsletter</h2>
            <p>Get the latest updates on new products and upcoming sales</p>
            <form className={styles.newsletterForm}>
              <input 
                type="email" 
                placeholder="Enter your email address" 
                className={styles.newsletterInput}
              />
              <button type="submit" className={styles.newsletterButton}>
                Subscribe
              </button>
            </form>
          </div>
        </div>
      </section>
    </>
  );
}

