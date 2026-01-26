import React from 'react';
import Link from 'next/link';
import { ChevronRightIcon } from '@/components/ui/Icons';
import styles from './SectionTitle.module.css';

interface SectionTitleProps {
  title: string;
  subtitle?: string;
  viewAllLink?: string;
  viewAllText?: string;
  centered?: boolean;
}

export default function SectionTitle({ 
  title, 
  subtitle, 
  viewAllLink, 
  viewAllText = 'View All',
  centered = false 
}: SectionTitleProps) {
  return (
    <div className={`${styles.sectionHeader} ${centered ? styles.centered : ''}`}>
      <div className={styles.titleWrapper}>
        <h2 className={styles.title}>{title}</h2>
        {subtitle && <p className={styles.subtitle}>{subtitle}</p>}
      </div>
      {viewAllLink && (
        <Link href={viewAllLink} className={styles.viewAllLink}>
          {viewAllText}
          <ChevronRightIcon size={18} />
        </Link>
      )}
    </div>
  );
}
