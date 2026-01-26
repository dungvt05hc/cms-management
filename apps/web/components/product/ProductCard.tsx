'use client';

import React from 'react';
import Link from 'next/link';
import { HeartIcon, CartIcon, EyeIcon, StarIcon } from '@/components/ui/Icons';
import styles from './ProductCard.module.css';

interface ProductVariant {
  id: string;
  price: number;
  stockQuantity: number;
}

interface Product {
  id: string;
  name: string;
  slug: string;
  description: string | null;
  images: string | null;
  isFeatured: boolean;
  variants: ProductVariant[];
}

interface ProductCardProps {
  product: Product;
  showQuickActions?: boolean;
}

export default function ProductCard({ product, showQuickActions = true }: ProductCardProps) {
  const minPrice = product.variants.length > 0 
    ? Math.min(...product.variants.map(v => v.price)) 
    : 0;
  
  const maxPrice = product.variants.length > 0 
    ? Math.max(...product.variants.map(v => v.price)) 
    : 0;

  const hasMultiplePrices = minPrice !== maxPrice;
  const isInStock = product.variants.some(v => v.stockQuantity > 0);

  // Parse images - assuming JSON array format
  const getFirstImage = (): string | null => {
    if (!product.images) return null;
    try {
      const images = JSON.parse(product.images);
      return Array.isArray(images) && images.length > 0 ? images[0] : null;
    } catch {
      return product.images;
    }
  };

  const imageUrl = getFirstImage();

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
    }).format(price);
  };

  return (
    <div className={styles.productCard}>
      <div className={styles.imageWrapper}>
        <Link href={`/products/${product.slug}`} className={styles.imageLink}>
          {imageUrl ? (
            <img src={imageUrl} alt={product.name} className={styles.productImage} />
          ) : (
            <div className={styles.imagePlaceholder}>
              <span>📦</span>
            </div>
          )}
        </Link>

        {/* Badges */}
        <div className={styles.badges}>
          {product.isFeatured && (
            <span className={styles.badgeFeatured}>Featured</span>
          )}
          {!isInStock && (
            <span className={styles.badgeOutOfStock}>Out of Stock</span>
          )}
        </div>

        {/* Quick Actions */}
        {showQuickActions && (
          <div className={styles.quickActions}>
            <button className={styles.actionBtn} title="Add to Wishlist">
              <HeartIcon size={18} />
            </button>
            <button className={styles.actionBtn} title="Quick View">
              <EyeIcon size={18} />
            </button>
            <button className={styles.actionBtn} title="Add to Cart">
              <CartIcon size={18} />
            </button>
          </div>
        )}
      </div>

      <div className={styles.productInfo}>
        {/* Rating */}
        <div className={styles.rating}>
          {[1, 2, 3, 4, 5].map((star) => (
            <StarIcon key={star} size={14} filled={star <= 4} color="#f59e0b" />
          ))}
          <span className={styles.ratingCount}>(24)</span>
        </div>

        {/* Product Name */}
        <h3 className={styles.productName}>
          <Link href={`/products/${product.slug}`}>{product.name}</Link>
        </h3>

        {/* Price */}
        <div className={styles.priceWrapper}>
          <span className={styles.price}>
            {hasMultiplePrices 
              ? `${formatPrice(minPrice)} - ${formatPrice(maxPrice)}`
              : formatPrice(minPrice)
            }
          </span>
        </div>

        {/* Add to Cart Button */}
        <button 
          className={styles.addToCartBtn}
          disabled={!isInStock}
        >
          <CartIcon size={16} />
          <span>{isInStock ? 'Add to Cart' : 'Out of Stock'}</span>
        </button>
      </div>
    </div>
  );
}
