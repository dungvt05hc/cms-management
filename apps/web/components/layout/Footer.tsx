import React from 'react';
import Link from 'next/link';
import {
  PhoneIcon,
  MailIcon,
  MapPinIcon,
  FacebookIcon,
  TwitterIcon,
  InstagramIcon,
  YoutubeIcon,
  TruckIcon,
  ShieldIcon,
  RefreshIcon,
  HeadphonesIcon
} from '@/components/ui/Icons';
import styles from './Footer.module.css';

export default function Footer() {
  return (
    <footer className={styles.footer}>
      {/* Features Bar */}
      <div className={styles.featuresBar}>
        <div className={styles.container}>
          <div className={styles.featuresGrid}>
            <div className={styles.featureItem}>
              <div className={styles.featureIcon}>
                <TruckIcon size={28} />
              </div>
              <div className={styles.featureContent}>
                <h4>Free Shipping</h4>
                <p>On orders over $50</p>
              </div>
            </div>
            <div className={styles.featureItem}>
              <div className={styles.featureIcon}>
                <RefreshIcon size={28} />
              </div>
              <div className={styles.featureContent}>
                <h4>Easy Returns</h4>
                <p>30 days return policy</p>
              </div>
            </div>
            <div className={styles.featureItem}>
              <div className={styles.featureIcon}>
                <ShieldIcon size={28} />
              </div>
              <div className={styles.featureContent}>
                <h4>Secure Payment</h4>
                <p>100% secure checkout</p>
              </div>
            </div>
            <div className={styles.featureItem}>
              <div className={styles.featureIcon}>
                <HeadphonesIcon size={28} />
              </div>
              <div className={styles.featureContent}>
                <h4>24/7 Support</h4>
                <p>Dedicated support team</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Main Footer */}
      <div className={styles.mainFooter}>
        <div className={styles.container}>
          <div className={styles.footerGrid}>
            {/* Company Info */}
            <div className={styles.footerColumn}>
              <Link href="/" className={styles.footerLogo}>
                <span className={styles.logoText}>CMS</span>
                <span className={styles.logoAccent}>Shop</span>
              </Link>
              <p className={styles.footerDescription}>
                Your one-stop destination for quality products at affordable prices. 
                We deliver excellence with every order.
              </p>
              <div className={styles.contactInfo}>
                <div className={styles.contactItem}>
                  <MapPinIcon size={16} />
                  <span>123 Commerce Street, City, Country</span>
                </div>
                <div className={styles.contactItem}>
                  <PhoneIcon size={16} />
                  <span>+1 (234) 567-8900</span>
                </div>
                <div className={styles.contactItem}>
                  <MailIcon size={16} />
                  <span>support@cmsshop.com</span>
                </div>
              </div>
            </div>

            {/* Quick Links */}
            <div className={styles.footerColumn}>
              <h3 className={styles.footerTitle}>Quick Links</h3>
              <ul className={styles.footerLinks}>
                <li><Link href="/products">Shop All</Link></li>
                <li><Link href="/products?featured=true">Featured Products</Link></li>
                <li><Link href="/deals">Hot Deals</Link></li>
                <li><Link href="/articles">Blog</Link></li>
                <li><Link href="/about">About Us</Link></li>
                <li><Link href="/contact">Contact Us</Link></li>
              </ul>
            </div>

            {/* Customer Service */}
            <div className={styles.footerColumn}>
              <h3 className={styles.footerTitle}>Customer Service</h3>
              <ul className={styles.footerLinks}>
                <li><Link href="/account">My Account</Link></li>
                <li><Link href="/account/orders">Order History</Link></li>
                <li><Link href="/wishlist">Wishlist</Link></li>
                <li><Link href="/track-order">Track Order</Link></li>
                <li><Link href="/help">Help Center</Link></li>
                <li><Link href="/faq">FAQ</Link></li>
              </ul>
            </div>

            {/* Policies */}
            <div className={styles.footerColumn}>
              <h3 className={styles.footerTitle}>Information</h3>
              <ul className={styles.footerLinks}>
                <li><Link href="/terms" data-testid="footer-link-terms">Terms of Service</Link></li>
                <li><Link href="/privacy">Privacy Policy</Link></li>
                <li><Link href="/sales-policy" data-testid="footer-link-sales-policy">Sales Policy</Link></li>
                <li><Link href="/shipping-policy">Shipping Policy</Link></li>
                <li><Link href="/return-policy">Return Policy</Link></li>
              </ul>
            </div>

            {/* Newsletter */}
            <div className={styles.footerColumn}>
              <h3 className={styles.footerTitle}>Newsletter</h3>
              <p className={styles.newsletterText}>
                Subscribe to get special offers, free giveaways, and new arrivals.
              </p>
              <form className={styles.newsletterForm}>
                <input 
                  type="email" 
                  placeholder="Enter your email" 
                  className={styles.newsletterInput}
                />
                <button type="submit" className={styles.newsletterButton}>
                  Subscribe
                </button>
              </form>
              <div className={styles.socialLinks}>
                <a href="https://facebook.com" target="_blank" rel="noopener noreferrer" className={styles.socialLink}>
                  <FacebookIcon size={20} />
                </a>
                <a href="https://twitter.com" target="_blank" rel="noopener noreferrer" className={styles.socialLink}>
                  <TwitterIcon size={20} />
                </a>
                <a href="https://instagram.com" target="_blank" rel="noopener noreferrer" className={styles.socialLink}>
                  <InstagramIcon size={20} />
                </a>
                <a href="https://youtube.com" target="_blank" rel="noopener noreferrer" className={styles.socialLink}>
                  <YoutubeIcon size={20} />
                </a>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Bottom Footer */}
      <div className={styles.bottomFooter}>
        <div className={styles.container}>
          <div className={styles.bottomContent}>
            <p className={styles.copyright}>
              © {new Date().getFullYear()} CMS Shop. All rights reserved.
            </p>
            <div className={styles.paymentMethods}>
              <span>We Accept:</span>
              <div className={styles.paymentIcons}>
                <span className={styles.paymentIcon}>Visa</span>
                <span className={styles.paymentIcon}>MasterCard</span>
                <span className={styles.paymentIcon}>PayPal</span>
                <span className={styles.paymentIcon}>COD</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </footer>
  );
}
