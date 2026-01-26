import React from 'react';
import Link from 'next/link';
import { ChevronRightIcon } from '@/components/ui/Icons';
import styles from './CategoryCard.module.css';

interface CategoryCardProps {
  id: string;
  name: string;
  productCount?: number;
  icon?: string;
}

const categoryIcons: Record<string, string> = {
  'electronics': '📱',
  'fashion': '👗',
  'home': '🏠',
  'beauty': '💄',
  'sports': '⚽',
  'toys': '🎮',
  'books': '📚',
  'food': '🍕',
  'default': '📦',
};

export default function CategoryCard({ id, name, productCount = 0, icon }: CategoryCardProps) {
  const displayIcon = icon || categoryIcons[name.toLowerCase()] || categoryIcons['default'];

  return (
    <Link href={`/products?categoryId=${id}`} className={styles.categoryCard}>
      <div className={styles.iconWrapper}>
        <span className={styles.icon}>{displayIcon}</span>
      </div>
      <div className={styles.content}>
        <h3 className={styles.name}>{name}</h3>
        {productCount > 0 && (
          <span className={styles.count}>{productCount} Products</span>
        )}
      </div>
      <ChevronRightIcon size={18} className={styles.arrow} />
    </Link>
  );
}
