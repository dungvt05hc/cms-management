'use client';

import React, { useState } from 'react';
import Link from 'next/link';
import { 
  SearchIcon, 
  UserIcon, 
  HeartIcon, 
  CartIcon, 
  MenuIcon, 
  CloseIcon,
  ChevronDownIcon,
  PhoneIcon,
  MapPinIcon
} from '@/components/ui/Icons';
import styles from './Header.module.css';

interface CategoryTreeNode {
  id: string;
  name: string;
  parentId: string | null;
  children: CategoryTreeNode[];
}

interface HeaderProps {
  categories?: CategoryTreeNode[];
}

export default function Header({ categories = [] }: HeaderProps) {
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const [activeDropdown, setActiveDropdown] = useState<string | null>(null);

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    if (searchQuery.trim()) {
      window.location.href = `/products?q=${encodeURIComponent(searchQuery)}`;
    }
  };

  return (
    <header className={styles.header}>
      {/* Top Bar */}
      <div className={styles.topBar}>
        <div className={styles.container}>
          <div className={styles.topBarContent}>
            <div className={styles.topBarLeft}>
              <div className={styles.topBarItem}>
                <PhoneIcon size={14} />
                <span>Hotline: 1900-1234</span>
              </div>
              <div className={styles.topBarItem}>
                <MapPinIcon size={14} />
                <span>Store Locator</span>
              </div>
            </div>
            <div className={styles.topBarRight}>
              <Link href="/help" className={styles.topBarLink}>Help Center</Link>
              <span className={styles.topBarDivider}>|</span>
              <Link href="/track-order" className={styles.topBarLink}>Track Order</Link>
              <span className={styles.topBarDivider}>|</span>
              <Link href="/account" className={styles.topBarLink}>My Account</Link>
            </div>
          </div>
        </div>
      </div>

      {/* Main Header */}
      <div className={styles.mainHeader}>
        <div className={styles.container}>
          <div className={styles.mainHeaderContent}>
            {/* Mobile Menu Toggle */}
            <button 
              className={styles.mobileMenuToggle}
              onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
              aria-label="Toggle menu"
            >
              {isMobileMenuOpen ? <CloseIcon size={24} /> : <MenuIcon size={24} />}
            </button>

            {/* Logo */}
            <Link href="/" className={styles.logo}>
              <span className={styles.logoText}>CMS</span>
              <span className={styles.logoAccent}>Shop</span>
            </Link>

            {/* Search Bar */}
            <form className={styles.searchBar} onSubmit={handleSearch}>
              <input
                type="text"
                placeholder="Search for products..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className={styles.searchInput}
              />
              <button type="submit" className={styles.searchButton}>
                <SearchIcon size={20} />
              </button>
            </form>

            {/* User Actions */}
            <div className={styles.userActions}>
              <Link href="/account" className={styles.actionButton} title="My Account">
                <UserIcon size={22} />
                <span className={styles.actionLabel}>Account</span>
              </Link>
              <Link href="/wishlist" className={styles.actionButton} title="Wishlist">
                <HeartIcon size={22} />
                <span className={styles.actionBadge}>0</span>
                <span className={styles.actionLabel}>Wishlist</span>
              </Link>
              <Link href="/cart" className={styles.actionButton} title="Cart">
                <CartIcon size={22} />
                <span className={styles.actionBadge}>0</span>
                <span className={styles.actionLabel}>Cart</span>
              </Link>
            </div>
          </div>
        </div>
      </div>

      {/* Navigation Bar */}
      <nav className={styles.navBar}>
        <div className={styles.container}>
          <div className={styles.navContent}>
            {/* Categories Dropdown */}
            <div 
              className={styles.categoriesDropdown}
              onMouseEnter={() => setActiveDropdown('categories')}
              onMouseLeave={() => setActiveDropdown(null)}
            >
              <button className={styles.categoriesButton}>
                <MenuIcon size={18} />
                <span>All Categories</span>
                <ChevronDownIcon size={16} />
              </button>
              {activeDropdown === 'categories' && categories.length > 0 && (
                <div className={styles.dropdownMenu}>
                  {categories.map((category) => (
                    <Link 
                      key={category.id} 
                      href={`/products?categoryId=${category.id}`}
                      className={styles.dropdownItem}
                    >
                      {category.name}
                      {category.children.length > 0 && (
                        <ChevronDownIcon size={14} className={styles.dropdownArrow} />
                      )}
                    </Link>
                  ))}
                </div>
              )}
            </div>

            {/* Main Navigation Links */}
            <ul className={styles.navLinks}>
              <li><Link href="/" className={styles.navLink}>Home</Link></li>
              <li><Link href="/products" className={styles.navLink}>Shop</Link></li>
              <li><Link href="/products?featured=true" className={styles.navLink}>Featured</Link></li>
              <li><Link href="/deals" className={styles.navLink}>Hot Deals</Link></li>
              <li><Link href="/articles" className={styles.navLink}>Blog</Link></li>
              <li><Link href="/contact" className={styles.navLink}>Contact</Link></li>
            </ul>

            {/* Promo Text */}
            <div className={styles.promoText}>
              <span className={styles.promoHighlight}>Free Shipping</span> on orders over $50
            </div>
          </div>
        </div>
      </nav>

      {/* Mobile Menu */}
      {isMobileMenuOpen && (
        <div className={styles.mobileMenu}>
          <div className={styles.mobileMenuContent}>
            <form className={styles.mobileSearch} onSubmit={handleSearch}>
              <input
                type="text"
                placeholder="Search..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
              />
              <button type="submit">
                <SearchIcon size={18} />
              </button>
            </form>
            <ul className={styles.mobileNavLinks}>
              <li><Link href="/">Home</Link></li>
              <li><Link href="/products">Shop</Link></li>
              <li><Link href="/products?featured=true">Featured</Link></li>
              <li><Link href="/deals">Hot Deals</Link></li>
              <li><Link href="/articles">Blog</Link></li>
              <li><Link href="/contact">Contact</Link></li>
            </ul>
            {categories.length > 0 && (
              <>
                <div className={styles.mobileMenuDivider} />
                <h4 className={styles.mobileMenuTitle}>Categories</h4>
                <ul className={styles.mobileNavLinks}>
                  {categories.map((category) => (
                    <li key={category.id}>
                      <Link href={`/products?categoryId=${category.id}`}>{category.name}</Link>
                    </li>
                  ))}
                </ul>
              </>
            )}
          </div>
        </div>
      )}
    </header>
  );
}
